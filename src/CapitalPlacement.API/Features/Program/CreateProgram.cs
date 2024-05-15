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
    public static class CreateProgram
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

                var newProgram = request.Adapt<EmployerProgram>();
                newProgram.id = Guid.NewGuid().ToString();

                foreach(var question in newProgram.questions)
                {
                    question.id = Guid.NewGuid().ToString();
                }

                IEnumerable<Question> baseQuestions = CreateBaseQuestions(newProgram.phoneInternal, newProgram.phoneHide,
                    newProgram.nationalityInternal, newProgram.nationalityHide,
                    newProgram.currentResidenceInternal, newProgram.currentResidenceHide,
                    newProgram.idNumberInternal, newProgram.idNumberInternal,
                    newProgram.dateOfBirthInternal, newProgram.dateOfBirthHide,
                    newProgram.genderInternal, newProgram.genderHide);

                newProgram.questions
                    .AddRange(baseQuestions);

                await _employerProgramRepository.CreateProgramAsync(newProgram);

                return Result<CreateProgramResponse>.Success(new CreateProgramResponse
                {
                    programId = newProgram.id
                });
            }

            private IEnumerable<Question> CreateBaseQuestions(bool phoneInternal, bool phoneHide,
                bool nationalityInternal, bool nationalityHide,
                bool currentResidenceInternal, bool currentResidenceHide,
                bool idNumberInternal, bool idNumberHide,
                bool dateOfBirthInternal, bool dateOfBirthHide,
                bool genderInternal, bool genderHide)
            {
                var result = new List<Question>
                {
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "First Name",
                        type = QuestionType.Paragraph,
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "Last Name",
                        type = QuestionType.Paragraph,
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "Email",
                        type = QuestionType.Paragraph,
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "Phone",
                        type = QuestionType.Paragraph,
                        hide = phoneHide,
                        internalProp = phoneInternal
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "Nationality",
                        type = QuestionType.Paragraph,
                        hide = nationalityHide,
                        internalProp = nationalityInternal
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "Current Residence",
                        type = QuestionType.Paragraph,
                        hide = currentResidenceHide,
                        internalProp = currentResidenceInternal
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "ID Number",
                        type = QuestionType.Paragraph,
                        hide = idNumberHide,
                        internalProp = idNumberInternal
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "Date of Birth",
                        type = QuestionType.Date,
                        hide = dateOfBirthHide,
                        internalProp = dateOfBirthInternal
                    },
                    new Question
                    {
                        id = Guid.NewGuid().ToString(),
                        questionText = "Gender",
                        type = QuestionType.Dropdown,
                        hide = genderHide,
                        internalProp = genderInternal,
                        choices = new() {"Male", "Female"},
                        enableOther = false
                    }
                };

                return result;
            }
        }
    }

    public class CreateProgramndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v1/employers/{employerid}/programs", async ([FromBody] CreateProgramDto request,
                [FromRoute] string employerid,
                ISender sender) =>
            {
                var command = request.Adapt<CreateProgram.Command>();
                command.employerid = employerid;

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
