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
    public class CreateProgram
    {
        public sealed class Command : IRequest<Result<CreateProgramResponse>>
        {
            public string title { get; set; }
            public string description { get; set; }
            public string employerid { get; set; }
            public bool phoneInternal { get; set; }
            public bool phoneHide { get; set; }
            public bool nationalityInternal { get; set; }
            public bool nationalityHide { get; set; }
            public bool currentResidenceInternal { get; set; }
            public bool currentResidenceHide { get; set; }
            public bool idNumberInternal { get; set; }
            public bool idNumberHide { get; set; }
            public bool dateOfBirthInternal { get; set; }
            public bool dateOfBirthHide { get; set; }
            public bool genderInternal { get; set; }
            public bool genderHide { get; set; }
            public List<QuestionDto> questions { get; set; }
        }

        public sealed class CreateProgramResponse
        {
            public string programId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {

            }
        }

        public sealed class Handler : IRequestHandler<Command, Result<CreateProgramResponse>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IEmployerProgramRepository _employerProgramRepository;

            public Handler(ILogger<Handler> logger, IEmployerProgramRepository employerProgramRepository)
            {
                _logger = logger;
                _employerProgramRepository = employerProgramRepository;
            }

            public async Task<Result<CreateProgramResponse>> Handle(Command request, CancellationToken cancellationToken)
            {

                var newProgram = new EmployerProgram(request.title, request.description, request.employerid);

                foreach (var question in request.questions)
                {
                    newProgram.AddQuestion(question.questionText, question.type,
                        question.choices, question.enableOther, question.maxChoices);
                }

                await _employerProgramRepository.CreateProgramAsync(newProgram);

                return Result<CreateProgramResponse>.Success(new CreateProgramResponse
                {
                    programId = newProgram.id
                });
            }
        }
    }

    public class CreateProgramndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/programs", async ([FromBody] CreateProgramDto request,
                ISender sender) =>
            {
                var command = request.Adapt<CreateProgram.Command>();

                var result = await sender.Send(command);

                return result.Match(
                    onSuccess: data => Results.Ok(ApiResponse<CreateProgram.CreateProgramResponse>.FromSuccess("Ok", data)),
                    onFailure: error => Results.UnprocessableEntity(error)
                    );
            })
                .WithTags("Programs")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Create a program"
                })
                .Produces<ApiResponse<CreateProgram.CreateProgramResponse>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<Shared.Error>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ErrorMessage>(StatusCodes.Status500InternalServerError);
        }
    }
}
