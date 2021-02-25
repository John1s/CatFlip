# CatFlip

This application returns a random example of a cat and performs to image transformations

## Cats

To get a picture of an upside down cat browse to:

    /api/cats

or use the Swagger UI.

    /api/docs/swagger

Additional optional transformation options can be included using query parameters

- tag   This allows you to only show cats that match the provided tab
- text  This allows you to display some additional text on you image (the right way around).
- scale This allows you to make the image larger or smaller, between 10% and 200%
- alpha This allows you to change the transparency of the image between 0-100%

For example:

    /api/cats?text=hello&tag=cute&scale=150&alpha=5

The transformation interface and transformation folder allows the addition of additional transformations as needed.

## API Documentation

The API documentation is provided by a swagger file within the application.
The swagger document is available at

    /api/docs/swagger/v1/swagger.json

In a development environment the swagger UI is available at

    /api/docs/swagger

## Database

The application supports an in memory or SqlServer database. To configure the database use the following setting in tha app.config file.

    "UseInMemoryDb" : "true"

For a SQLServer database a connection string must also be provided.

    "ConnectionStrings": {
      "UserDatabase": "Server=localhost,1433;Database=UserDb;User ID=sa;Password="
    }

## Authentication

The application supports Basic and OpenId authentication.

### Basic

For basic authentication create a user using the

    POST /api/user

Then authenticate using

    POST /api/auth/login

### OpenId

You login with Open ID enter the details for the identity provider in the appsettings.json file.

  "Openid": {
    "Authority": "",
    "ClientSecret": "",
    "ClientId": ""
  }

The client secret can be provided by using an environment variable. An example configuration is included in the appsettings.Development.json file.
This uses an demo client on the https://auth0.com/ site. Users can authenticate using:

Username: test1@demo.com
Password: Password!

You can use the external identity provider by browing directly to the login URL and be redirected directly back to the page you want. Allowing you to access your cat pictures as quickly as possible.

    /api/auth/login?redirecturi=/api/cats

Note: The demo only the application running locally on ports 5001 or 44395.
http://localhost:44395
http://localhost:5001

Both authentication methods set an authentication cookie in the browser. This will allow yout to view your favourite cats using the Swagger UI or directly in the browser regardless of the authentication option you choose.

### Logout

To logout browse to

    /api/auth/logiout

If you logged in using an external identity provider you will be redirect to loggout their also if the provider supports the end session endpoint.
