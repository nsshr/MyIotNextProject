FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# EXPOSE 5000

WORKDIR /app

COPY IotProject.csproj ./

RUN dotnet restore

RUN dotnet dev-certs https --clean

RUN dotnet dev-certs https --trust

COPY . .

RUN dotnet publish -c Release -o out

# # # Build runtime image

# FROM mcr.microsoft.com/dotnet/aspnet:5.0

# WORKDIR /app

# EXPOSE 5000

# COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "watch", "run", "--no-restore", "--urls", "http://0.0.0.0:5000"]
