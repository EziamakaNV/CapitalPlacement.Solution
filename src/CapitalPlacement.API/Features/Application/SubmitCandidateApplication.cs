using CapitalPlacement.API.Contracts;
using CapitalPlacement.API.Exceptions;
using CapitalPlacement.API.Features.Program;
using CapitalPlacement.API.Interfaces;
using CapitalPlacement.API.Models;
using CapitalPlacement.API.Shared;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static CapitalPlacement.API.Middleware.ExceptionMiddleware;

namespace CapitalPlacement.API.Features.Application
{
    public static class SubmitCandidateApplication
    {
        public sealed class Command : IRequest<Result<SubmitApplicationResponseDto>>
        {
            public string employerid { get; set; }
            public string programid { get; set; }
            public List<AnswerCommandDto> answers { get; set; }
        }

        public class AnswerCommandDto
        {
            public string questionid { get; set; }
            public string answerText { get; set; }
        }

        public sealed class SubmitApplicationResponseDto
        {
            public string Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {

            }
        }

        public sealed class Handler : IRequestHandler<Command, Result<SubmitApplicationResponseDto>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IEmployerProgramRepository _employerProgramRepository;
            private readonly ICandidateApplicationRepository _candidateApplicationRepository;

            public Handler(ILogger<Handler> logger, IEmployerProgramRepository employerProgramRepository,
                ICandidateApplicationRepository candidateApplicationRepository)
            {
                _logger = logger;
                _employerProgramRepository = employerProgramRepository;
                _candidateApplicationRepository = candidateApplicationRepository;
            }

            public async Task<Result<SubmitApplicationResponseDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                
                var newApplicationId = Guid.NewGuid().ToString();

                var candidateApplication = request.Adapt<CandidateApplication>();
                candidateApplication.id = newApplicationId;

                await _candidateApplicationRepository.AddAsync(candidateApplication);

                return Result<SubmitApplicationResponseDto>.Success(new SubmitApplicationResponseDto
                {
                    Id = newApplicationId
                });
            }
        }
    }
    public class SubmitCandidateApplicationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/employers/{employerid}/programs/{programid}/submit-application", async ([FromBody] SubmitApplicationDto request,
                [FromRoute] string employerid,
                [FromRoute] string programid,
                ISender sender) =>
            {
                var command = request.Adapt<SubmitCandidateApplication.Command>();
                command.employerid = employerid;
                command.programid = programid;

                var result = await sender.Send(command);

                return result.Match(
                    onSuccess: data => Results.Ok(ApiResponse<SubmitCandidateApplication.SubmitApplicationResponseDto>.FromSuccess("Ok", data)),
                    onFailure: error => Results.UnprocessableEntity(error)
                    );
            })
                .WithTags("Candidate Application")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Submit an application"
                })
                .Produces<ApiResponse<SubmitCandidateApplication.SubmitApplicationResponseDto>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<Shared.Error>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ErrorMessage>(StatusCodes.Status500InternalServerError);
        }
    }

}
