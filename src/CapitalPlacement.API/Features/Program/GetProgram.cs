using CapitalPlacement.API.Contracts;
using CapitalPlacement.API.Exceptions;
using CapitalPlacement.API.Interfaces;
using CapitalPlacement.API.Models;
using CapitalPlacement.API.Shared;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static CapitalPlacement.API.Middleware.ExceptionMiddleware;

namespace CapitalPlacement.API.Features.Program
{
    public static class GetProgram
    {
        public sealed class Query : IRequest<Result<GetProgramDto>>
        {
            public string employerid { get; set; }
            public string programid { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {

            }
        }

        public sealed class Handler : IRequestHandler<Query, Result<GetProgramDto>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IEmployerProgramRepository _employerProgramRepository;

            public Handler(ILogger<Handler> logger, IEmployerProgramRepository employerProgramRepository)
            {
                _logger = logger;
                _employerProgramRepository = employerProgramRepository;
            }

            public async Task<Result<GetProgramDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var program = await _employerProgramRepository.GetProgramByIdAsync(request.programid, request.employerid);

                if (program is null)
                {
                    return Result<GetProgramDto>.Failure(new Error("GetProgram.Error,", "The program does not exist"));
                }

                var programDto = program.Adapt<GetProgramDto>();


                return Result<GetProgramDto>.Success(programDto);
            }
        }
    }

    public class GetProgramndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/employers/{employerid}/programs/{programid}", async ([FromRoute] string employerid,
                [FromRoute] string programid,
                ISender sender) =>
            {
                var query = new GetProgram.Query();
                query.employerid = employerid;
                query.programid = programid;

                var result = await sender.Send(query);

                return result.Match(
                    onSuccess: data => Results.Ok(ApiResponse<GetProgramDto>.FromSuccess("Ok", data)),
                    onFailure: error => Results.UnprocessableEntity(error)
                    );
            })
                .WithTags("Programs")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Get a program"
                })
                .Produces<ApiResponse<GetProgramDto>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<Shared.Error>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ErrorMessage>(StatusCodes.Status500InternalServerError);
        }
    }
}
