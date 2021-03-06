#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["RedditAnswerGenerator/RedditAnswerGenerator.csproj", "RedditAnswerGenerator/"]
COPY ["RedditAnswerGenerator.Services/RedditAnswerGenerator.Services.csproj", "RedditAnswerGenerator.Services/"]
RUN dotnet restore "RedditAnswerGenerator/RedditAnswerGenerator.csproj"
COPY . .
WORKDIR "/src/RedditAnswerGenerator"
RUN dotnet build "RedditAnswerGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedditAnswerGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedditAnswerGenerator.dll"]