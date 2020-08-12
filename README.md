# Keycloak implementation with a .Net Core

Demo to show the implementation of oAuth2 and OIDC standards through keycloak and .Net Core

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- **Docker** - [Install guide](https://docs.docker.com/docker-for-windows/install/)
- **Keycloak** - [Getting started guide](https://www.keycloak.org/getting-started/getting-started-docker)
- **JSON Web Token** - [Guide](https://jwt.io/introduction/)
- **oAuth2 and OpenID Connect** - [Video guide](https://app.pluralsight.com/library/courses/oauth2-json-web-tokens-openid-connect-introduction/table-of-contents?aid=7010a000002LUv2AAG)

From a terminal(CMD) start Keycloak with the following command:

```
docker run -p 8080:8080 -e KEYCLOAK_USER=admin -e KEYCLOAK_PASSWORD=admin quay.io/keycloak/keycloak:11.0.0
```

This will start Keycloak exposed on the local port 8080. It will also create an initial admin user with username `admin` and password `admin`.

# Keycloak configuration

Explain how to setup keycloak to work with this demo.

Open and login to admin console in browser

```
http://localhost:8080/auth/admin
```

Don't change the realm for simplicity, just stay on the master.

## Client setup

Secure our applications from demo (WebAppDemo, WebApiDemo and WebAppSSO). First step is to Register our applications with keycloak instance.

Create and Setup _WebAppDemo_ Client

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click `Create` (top-right conter of table)
3. Fill in the form with the following values
   - Client ID: `WebAppDemo`
   - Client Protocol: `openid-connect`
   - Root URL: `http://localhost:1214/`(Put the localhost port assigned to you by the visual studio when you start the project)
4. Click `Save`
5. In settings form change following value
   - Client Protocol: `openid-connect`
   - Access Type: `confidential`
   - Standard Flow Enabled: `ON`
   - Root URL: `http://localhost:1214/`
   - Valid Redirect URIs: `*`
   - Base URL: `http://localhost:8080`
   - Admin: `http://localhost:8080`
   - Web Origins: `*`
6. Click `Save`

Create and Setup _WebAppDemoSSO_ Client

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click `Create` (top-right conter of table)
3. Fill in the form with the following values
   - Client ID: `WebAppDemoSSO`
   - Client Protocol: `openid-connect`
   - Root URL: `http://localhost:44384/`(Put the localhost port assigned to you by the visual studio when you start the project)
4. Click `Save`
5. In settings form change following value
   - Client Protocol: `openid-connect`
   - Access Type: `confidential`
   - Standard Flow Enabled: `ON`
   - Root URL: `http://localhost:44384/`
   - Valid Redirect URIs: `*`
   - Base URL: `http://localhost:8080`
   - Admin: `http://localhost:8080`
   - Web Origins: `*`
6. Click `Save`

Create and Setup _WebApiDemo_ Client

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click `Create` (top-right conter of table)
3. Fill in the form with the following values
   - Client ID: `WebApiDemo`
   - Client Protocol: `openid-connect`
   - Root URL: `https://localhost:44373/weatherforecast`(Put the localhost port assigned to you by the visual studio when you start the project)
4. Click `Save`
5. In settings form change following values
   - Client Protocol: `openid-connect`
   - Access Type: `bearer-only`
   - Admin URL: `https://localhost:44373/weatherforecast`
6. Click `Save`

## Users Setup

Initially there are no users in realm except admin realm user which we added with keycloak docker image.

Create and setup `apiAdmin` user

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Users` (left-hand menu)
   - Click `Add user` (top-right conter of table)
3. Fill in the form with the following values
   - Username: `apiAdmin`
   - Email: `apiAdmin@mail.com`
   - First Name: `api`
   - Last Name: `admin`
   - User Enabled: `ON`
   - Email Verified: `ON`
4. Click `Save`
5. Click `Credentials`
6. Fill in the `Set Password` form with a password
7. Click `ON` next to `Temporary` to prevent having to update password on first login

Create and setup `user` user

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Users` (left-hand menu)
   - Click `Add user` (top-right conter of table)
3. Fill in the form with the following values
   - Username: `user`
   - Email: `user@mail.com`
   - First Name: `user`
   - Last Name: `user`
   - User Enabled: `ON`
   - Email Verified: `ON`
4. Click `Save`
5. Click `Credentials`
6. Fill in the `Set Password` form with a password
7. Click `ON` next to `Temporary` to prevent having to update password on first login
8. Click `Attributes` to add new custome claim
9. Fill in `Key` form with a value `OIB` and `Value` form with a value `1234567890`

## Adding Roles

Our _WebApiDemo_ have 2 Authorization Policy which expect `apiuser` or `apiAdmin` roles.

Create and assign `apiAdmin` role

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click Client ID `WebApiDemo`
3. Click `Roles`
   - Click `Add Role`
4. Fill in the `Role Name` with a `apiAdmin`
5. Now to assign role click on `Users`
   - Click `Edit` on user `apiAdmin`
   - Click `Role Mappings`
   - Select `WebApiDemo` where we put roles on and assgine `apiuser` and `apiAdmin` roles to `Assigned Roles`

Create and assign `apiuser` role

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click Client ID `WebApiDemo`
3. Click `Roles`
   - Click `Add Role`
4. Fill in the `Role Name` with a `apiuser`
5. Now to assign role click on `Users`
   - Click `Edit` on user `user`
   - Click `Role Mappings`
   - Select `WebApiDemo` where we put roles on and assgine `apiuser` role to `Assigned Roles`

## Adding Mappers

To be able to forward to the client custome attribute `OIB` which we added to `user` and users roles we need tell keycloak to put it into access token

Create User Attribute mapper

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click Client ID `WebAppDemo`
3. Click `Mappers`
   - Click `Create`
4. Fill in the form with the following values
   Name: `oib`
   Mapper Type: `User Attribute`
   User Attribute: `oib`
   Token Claim Name: `oib`
   Claim JSON Type: `String`
   Add to ID token: `ON`
   Add to access token: `ON`
   Add to userinfo: `ON`
5. Click `Save`

Create User Client Role mapper

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click Client ID `WebAppDemo`
3. Click `Mappers`
   - Click `Create`
4. Fill in the form with the following values
   Name: `User client roles`
   Mapper Type: `User Client Role`
   Client ID: `WebApiDemo`
   Token Claim Name: `user_roles`
   Claim JSON Type: `String`
   Add to ID token: `ON`
   Add to access token: `ON`
   Add to userinfo: `ON`
5. Click `Save`

# Applications configuration

Next step is to setup our applications to know how to communicate with keycloak. Need to provide client secret which keycloak create for us and give it to our applications

1. Open the [Keycloak Admin Console](http://localhost:8080/auth/admin)
2. Click `Clients` (left-hand menu)
   - Click Client ID `WebAppDemo`
   - Click Credentials and Copy `Secret`
3. Open Visual Studio and load project
4. In _WebAppDemo_ and _WebAppDemoSSO_ open `appsettings.json`
   - Paste `Secret` from Keycloak to field `ClientSecret`

# Running the tests

Application is ready for test now, only one disclaimer before starting solution, you need setup all 3 project to start, [here](https://docs.microsoft.com/en-us/visualstudio/ide/how-to-set-multiple-startup-projects?view=vs-2019) you can find help.
