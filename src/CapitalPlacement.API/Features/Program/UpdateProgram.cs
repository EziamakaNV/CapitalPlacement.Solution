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
    public static class UpdateProgram
    {
        public sealed class Command : IRequest<Result>
        {
            public string title { get; set; }
            public string description { get; set; }
            public string employerid { get; set; }
            public string programid { get; set; }
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

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {

            }
        }

        public sealed class Handler : IRequestHandler<Command, Result>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IEmployerProgramRepository _employerProgramRepository;

            public Handler(ILogger<Handler> logger, IEmployerProgramRepository employerProgramRepository)
            {
                _logger = logger;
                _employerProgramRepository = employerProgramRepository;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var program = await _employerProgramRepository.GetProgramByIdAsync(request.programid, request.employerid);

                if (program is null)
                {
                    return Result.Failure(new Error("UpdateProgam.Error","The program does not exist"));
                }

                var updatedProgram = request.Adapt<EmployerProgram>();
                updatedProgram.id = program.id;
                updatedProgram.employerid = program.employerid;

                foreach (var question in updatedProgram.questions)
                {
                    question.id = Guid.NewGuid().ToString();
                }

                IEnumerable<Question> baseQuestions = CreateBaseQuestions(updatedProgram.phoneInternal, updatedProgram.phoneHide,
                    updatedProgram.nationalityInternal, updatedProgram.nationalityHide,
                    updatedProgram.currentResidenceInternal, updatedProgram.currentResidenceHide,
                    updatedProgram.idNumberInternal, updatedProgram.idNumberInternal,
                    updatedProgram.dateOfBirthInternal, updatedProgram.dateOfBirthHide,
                    updatedProgram.genderInternal, updatedProgram.genderHide);

                updatedProgram.questions
                    .AddRange(baseQuestions);

                await _employerProgramRepository.UpdateProgramAsync(updatedProgram, request.employerid);

                return Result.Success();
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

    public class UpdateProgramndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v1/employers/{employerid}/programs/{programid}", async ([FromBody] UpdateProgramDto request,
                [FromRoute] string employerid,
                [FromRoute] string programid,
                ISender sender) =>
            {
                var command = request.Adapt<UpdateProgram.Command>();
                command.employerid = employerid;
                command.programid = programid;

                var result = await sender.Send(command);

                return result.Match(
                    onSuccess: () => Results.NoContent(),
                    onFailure: error => Results.UnprocessableEntity(error)
                    );
            })
                .WithTags("Programs")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Update a program"
                })
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                .Produces<Shared.Error>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ErrorMessage>(StatusCodes.Status500InternalServerError);
        }
    }
}
