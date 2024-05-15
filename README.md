# CapitalPlacement API

## Overview

The CapitalPlacement API is a .NET 8 Web API application designed to manage employer programs and candidate applications. Employers can create programs with custom questions, and candidates can apply to these programs by answering the questions. The application leverages Azure Cosmos DB for data storage and runs on Docker Compose for containerized deployment.

## General Flow

1. **Employers**:
   - **Create Program**: Employers can create a program with mandatory fields such as title and description, along with custom questions (e.g., Paragraph, Yes/No, Dropdown, Multiple Choice, Date, Number).
   - **Edit Program**: Employers can edit the program details and questions after creation.

2. **Candidates**:
   - **View Program**: Candidates can view the list of questions for a program.
   - **Apply to Program**: Candidates can submit their application by answering the questions. The application captures mandatory fields such as first name, last name, and email, ensuring that the email is unique.

## Setup and Configuration

### Prerequisites

- Docker and Docker Compose installed
- .NET 8 SDK installed

### Running the Application

1. **Clone the Repository**

   ```bash
   git clone https://github.com/your-repo/capitalplacement-api.git
   cd capitalplacement-api
   ```

2. **Set Up Docker Compose**

   The application and the Azure Cosmos DB emulator are configured to run using Docker Compose. 

3. **Build and Run the Services**

   ```bash
   docker-compose up --build
   ```

   This command will build the Docker images and start the services defined in the `docker-compose.yml` file.

   ## API Endpoints

### Submit an Application
- **POST** `/api/v1/employers/{employerid}/programs/{programid}/submit-application`
  - **Parameters**: `employerid`, `programid`
  - **Responses**: `200 OK`, `400 Bad Request`, `422 Unprocessable Entity`, `500 Internal Server Error`

### Create a Program
- **POST** `/api/v1/employers/{employerid}/programs`
  - **Parameters**: `employerid`
  - **Responses**: `200 OK`, `400 Bad Request`, `422 Unprocessable Entity`, `500 Internal Server Error`

### Get a Program
- **GET** `/api/v1/employers/{employerid}/programs/{programid}`
  - **Parameters**: `employerid`, `programid`
  - **Responses**: `200 OK`, `400 Bad Request`, `422 Unprocessable Entity`, `500 Internal Server Error`

### Update a Program
- **PUT** `/api/v1/employers/{employerid}/programs/{programid}`
  - **Parameters**: `employerid`, `programid`
  - **Responses**: `204 No Content`, `400 Bad Request`, `422 Unprocessable Entity`, `500 Internal Server Error`

## Potential Enhancements

Given extra time, several improvements could be made to enhance the functionality and robustness of the application:

1. **Validation of DTOs**: Implement comprehensive validation for Data Transfer Objects (DTOs) to ensure data integrity and provide meaningful error messages to users.
2. **Domain Logic Checks**:
   - **Ensure a Candidate Can Apply More Than Once**: Implement logic to prevent multiple applications from the same candidate to the same program.
   - **Ensure All Required Questions Are Answered**: Validate that all mandatory questions are answered before accepting an application.
   - **Ensure Answers Match Question Type**: Check that the answers provided by candidates match the expected type of each question.
   - **Check Questions Exist Before Accepting an Answer**: Verify that all questions referenced in the application exist in the program before accepting the answers.

These enhancements would ensure that the application is more robust, user-friendly, and reliable, providing a better experience for both employers and candidates.