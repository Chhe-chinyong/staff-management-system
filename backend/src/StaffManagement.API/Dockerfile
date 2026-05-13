FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/StaffManagement.Domain/StaffManagement.Domain.csproj src/StaffManagement.Domain/
COPY src/StaffManagement.Application/StaffManagement.Application.csproj src/StaffManagement.Application/
COPY src/StaffManagement.Infrastructure/StaffManagement.Infrastructure.csproj src/StaffManagement.Infrastructure/
COPY src/StaffManagement.API/StaffManagement.API.csproj src/StaffManagement.API/
RUN dotnet restore src/StaffManagement.API/StaffManagement.API.csproj

COPY . .
RUN dotnet publish src/StaffManagement.API/StaffManagement.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Docker
EXPOSE 5000

ENTRYPOINT ["dotnet", "StaffManagement.API.dll", "--urls", "http://0.0.0.0:5000"]
