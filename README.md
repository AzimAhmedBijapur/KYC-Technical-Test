# KYC C# Developer Technical Test

Name: Azim Ahmed Bijapur		

## Aim : To develop a Rest API using ASP.NET Core Web API

## Database Model:

I have implemented a model named ‘Entity’ with the schema as provided in the assignment document.

## Mock database implementation:

In order to implement a mocked database I have used a repository pattern. First I implemented an IEntityRepository interface declaring the methods and attributes. The interface is then implemented by MockEntityRepository. Both of which are present in the folder ‘Repositories’.

## API Endpoints:

I have implemented an EntitiesController which handles all the requests to the server. It includes the following functions:-

- Create

- Update

- Delete

- Read or Fetch

  - Single entity - get by ID

  - List of Entities

    This endpoint does the following:

    - Fetch all the entities in the repository
    - Search by address or name
    - Filter by gender, date and countries
    - Implements pagination and sorting by ID
	
Logic for sorting is implemented in IQueryableExtension.cs


## Retry ,back-off and logging mechanism:

Implemented a retry mechanism on Post and Put requests. The function attempts to perform a write operation 3 times. In case of transient errors, before re-attempting it waits for a certain ‘delay’ time which increases exponentially after every attempt. Relevant information about no. of retry attempts and delay is logged using the LogRetryAttempt() function. The above functions are implemented in the MockEntityRepository.cs.

## Testing:

Tests have been implemented for verifying whether add and update operations are working as expected, coupled with the Retry and Back-off mechanism.










