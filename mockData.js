const database = "TesfaFundDb";

use(database);

db.Recipients.insertMany([
  {
    _id: "a1b2c3d4-e5f6-7890-1234-567890abcdef",
    FirstName: "John",
    MiddleName: "Michael",
    LastName: "Doe",
    Email: "john.doe@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "b2c3d4e5-f6a7-8901-2345-67890abcdef0",
    FirstName: "Jane",
    LastName: "Smith",
    Email: "jane.smith@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "c3d4e5f6-a7b8-9012-3456-7890abcdef1",
    FirstName: "Bob",
    MiddleName: "James",
    LastName: "Johnson",
    Email: "bob.johnson@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "d4e5f6a7-b8c9-0123-4567-890abcdef222",
    FirstName: "Alice",
    LastName: "Williams",
    Email: "alice.williams@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "e5f6a7b8-c9d0-1234-5678-90a222bcdef3",
    FirstName: "Mike",
    MiddleName: "Robert",
    LastName: "Brown",
    Email: "mike.brown@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "f6a7b8c9-d0e1-2345-6789-0abcfed3def4",
    FirstName: "Sarah",
    LastName: "Taylor",
    Email: "sarah.taylor@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "a7b8c9d0-e1f2-3456-7890-abcdef523ea7",
    FirstName: "David",
    LastName: "Anderson",
    Email: "david.anderson@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "b8c9d0e1-f2a3-4567-890a-bcdef0ab4812",
    FirstName: "Emily",
    LastName: "Thomas",
    Email: "emily.thomas@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "c9d0e1f2-a3b4-5678-90ab-cdef0123cdef",
    FirstName: "Chris",
    LastName: "Jackson",
    Email: "chris.jackson@example.com",
    PasswordHash: "hashedpassword",
  },
  {
    _id: "d0e1f2a3-b4c5-6789-0abc-def01234cdef",
    FirstName: "Laura",
    LastName: "White",
    Email: "laura.white@example.com",
    PasswordHash: "hashedpassword",
  },
]);
db.Campaigns.insertMany([
  {
    _id: "123e4567-e89b-12d3-a456-426655440000",
    Title: "Help John's Family",
    Description:
      "John's family is in need of financial assistance due to a recent medical emergency.",
    FundraisingGoal: 10000,
    RecipientId: "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  },
  {
    _id: "234e5678-e89b-12d3-a456-426655440001",
    Title: "Support Local Animal Shelter",
    Description:
      "Our local animal shelter is in need of funds to care for the animals and provide them with a safe environment.",
    FundraisingGoal: 5000,
    RecipientId: "b2c3d4e5-f6a7-8901-2345-67890abcdef0",
  },
  {
    _id: "345e6789-e89b-12d3-a456-426655440002",
    Title: "Help Sarah's Education",
    Description:
      "Sarah is a bright student who needs financial assistance to pursue her higher education.",
    FundraisingGoal: 20000,
    RecipientId: "f6a7b8c9-d0e1-2345-6789-0abcfed3def4",
  },
  {
    _id: "456e7890-e89b-12d3-a456-426655440003",
    Title: "Disaster Relief Fund",
    Description:
      "A recent natural disaster has affected many families, and we need your help to provide them with the necessary assistance.",
    FundraisingGoal: 50000,
    RecipientId: "a7b8c9d0-e1f2-3456-7890-abcdef523ea7",
  },
  {
    _id: "567e8901-e89b-12d3-a456-426655440004",
    Title: "Community Development Project",
    Description:
      "We are working on a community development project that aims to improve the lives of the people in our community.",
    FundraisingGoal: 30000,
    RecipientId: "b8c9d0e1-f2a3-4567-890a-bcdef0ab4812",
  },
]);
db.Donations.insertMany([
  {
    _id: "123e4567-e89b-12d3-a456-426655440000",
    Amount: 100,
    TimeStamp: ISODate("2022-01-01T12:00:00.000Z"),
    CampaignId: "123e4567-e89b-12d3-a456-426655440000",
  },
  {
    _id: "234e5678-e89b-12d3-a456-426655440001",
    Amount: 200,
    TimeStamp: ISODate("2022-01-02T13:00:00.000Z"),
    CampaignId: "123e4567-e89b-12d3-a456-426655440000",
  },
  {
    _id: "345e6789-e89b-12d3-a456-426655440002",
    Amount: 50,
    TimeStamp: ISODate("2022-01-03T14:00:00.000Z"),
    CampaignId: "234e5678-e89b-12d3-a456-426655440001",
  },
  {
    _id: "456e7890-e89b-12d3-a456-426655440003",
    Amount: 150,
    TimeStamp: ISODate("2022-01-04T15:00:00.000Z"),
    CampaignId: "345e6789-e89b-12d3-a456-426655440002",
  },
  {
    _id: "567e8901-e89b-12d3-a456-426655440004",
    Amount: 250,
    TimeStamp: ISODate("2022-01-05T16:00:00.000Z"),
    CampaignId: "456e7890-e89b-12d3-a456-426655440003",
  },
]);
