# Crowdfunding Platform API

This project implements a RESTful API for a crowdfunding platform using .NET 6, MongoDB, and Docker. It provides functionality for managing recipients, campaigns, and donations.

## Features

This API provides the backend functionality for a crowdfunding platform. It allows users (recipients) to create and manage campaigns, and donors to contribute to these campaigns. The API utilizes MongoDB for data persistence and incorporates best practices such as dependency injection, clear separation of concerns, and robust error handling. Key features include:

- **Recipient Management:** Recipient registration, login, and logout.
- **Campaign Management:** Campaign creation, retrieval (with optional search and filtering), updates, deletion, and tracking of donation progress. Campaign creation, updates, and deletion are restricted to the campaign's recipient.
- **Donation Management:** Making donations; retrieving donation details. Donations are immutable (cannot be modified or deleted once created).
- **API Documentation:** Interactive API documentation using Swagger.

## Technologies Used

- .NET 6
- MongoDB
- Swashbuckle (for Swagger)
- Docker

## Running the API

- Without Docker

  1. Restore NuGet packages: `dotnet restore`
  2. Build the project: `dotnet build`
  3. Run the API: `dotnet run`

- With Docker
  1. Build the Docker image: `docker build -t tesfafund-api .`
  2. Run the Docker container: `docker run -p 5000:5000 tesfafund-api`

## API Endpoints

### Recipients

- [x] (hemen) - `POST /api/recipients`: Create a new recipient.
- [x] (hemen) - `GET /api/recipients/{id}`: Get a recipient by ID.
- [x] (bisrat) - `PUT /api/recipients/{id}`: Update a recipient.
- [x] (bisrat) - `DELETE /api/recipients/{id}`: Delete a recipient.
- [x] (bitbender-8) - `GET - /api/recipients`: Get all recipients (with optional search and filter params).

### Campaigns

Campaigns can be created, updated, or deleted.

- [x] (blackmammoth) - `POST /api/campaigns`: Create a new campaign.
- [x] (blackmammoth) - `PUT /api/campaigns/{id}`: Update a campaign.
- [x] (bitbender-8) - `DELETE /api/campaigns/{id}`: Delete a campaign.
- [x] (bisrat) - `GET /api/campaigns/{id}`: Get a campaign by ID.
- [x] (bitbender-8) - `GET /api/campaigns`: Get all campaigns (with optional search and filtering).
- [x] (bitbender-8) - `GET /api/campaigns/{id}/progress`: Get donation progress for a campaign.

### Donations

Donations are immutable. Once created, they cannot be modified or deleted.

- [x] (blackmammoth) - `GET /api/donations/{id}`: Get donation details.
- [x] (blackmammoth) - `POST /api/donations`: Make a donation.
- [x] (bitbender-8) - `GET /api/donations`: Get donations (with search and filter params).

## Advanced Features

- **Search and Filtering:** Implemented for Campaigns, Users, Recipients.
- **Dockerization:** The application can be easily containerized and deployed using Docker.
- **Validation:** Validation using attributes on entity classes.
