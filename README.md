# Crowdfunding Platform API

This project implements a RESTful API for a crowdfunding platform using .NET 6, MongoDB, and Docker. It provides functionality for managing recipients, campaigns, and donations.

## Features

- **CRUD Operations:**  Create, Read, Update, and Delete functionality for Recipients, Campaigns, and Donations.
- **Recipient Management:**  Recipient registration, login, logout.
- **Campaign Management:**  Campaign creation, retrieval (with search and filtering), updates, deletion, and tracking donation progress.
- **Donation Management:**  Making donations, retrieving donation details.
- **Input Validation:**  Data validation using Data Annotations to ensure data integrity.
- **Error Handling:**  Global exception handling with appropriate HTTP status codes.
- **Search and Filtering:**  Search campaigns by title, category, etc.
- **Pagination:**  Pagination for large datasets (e.g., listing campaigns).
- **Dockerization:**  Easy deployment and consistent environment using Docker.
- **API Documentation:**  Interactive API documentation using Swagger.

## Technologies Used

- .NET 6
- MongoDB
- Swashbuckle (for Swagger)
- Docker

### Running the API

Option 1: Without Docker

1. Restore NuGet packages: `dotnet restore`
2. Build the project: `dotnet build`
3. Run the API: `dotnet run`

Option 2: With Docker

1. Build the Docker image: `docker build -t crowdfunding-api .`
2. Run the Docker container: `docker run -p 8080:8080 crowdfunding-api`

## API Endpoints

### Recipients

- `POST /api/recipients`: Create a new recipient.
- `GET /api/recipients/{id}`: Get a recipient by ID.
- `PUT /api/recipients/{id}`: Update a recipient.
- `DELETE /api/recipients/{id}`: Delete a recipient.
- `POST /api/recipients/login`: Recipient login.
- `POST /api/recipients/logout`: Recipient logout.

### Campaigns

- `POST /api/campaigns`: Create a new campaign.
- `GET /api/campaigns`: Get all campaigns (with optional search and filtering).
- `GET /api/campaigns/{id}`: Get a campaign by ID.
- `PUT /api/campaigns/{id}`: Update a campaign.
- `DELETE /api/campaigns/{id}`: Delete a campaign.
- `GET /api/campaigns/{id}/progress`: Get donation progress for a campaign.

### Donations

- `POST /api/donations`: Make a donation.
- `GET /api/donations/{id}`: Get donation details.

## Advanced Features

- **Search and Filtering:** Implemented for Campaigns.
- **Pagination:** Implemented for listing Campaigns.
- **Dockerization:** The application can be easily containerized and deployed using Docker.
- **Input Validation:** Data annotations are used for input validation.
