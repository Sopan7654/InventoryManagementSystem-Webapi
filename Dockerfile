# ─────────────────────────────────────────────────────────────────────────────
# Stage 1: Build — restore & compile in the full SDK image
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the project file first so NuGet restore is cached as a separate layer
COPY ["src/InventoryManagementSystem/InventoryManagementSystem.csproj", \
      "src/InventoryManagementSystem/"]

RUN dotnet restore "src/InventoryManagementSystem/InventoryManagementSystem.csproj"

# Copy the rest of the source
COPY src/ src/

WORKDIR "/src/src/InventoryManagementSystem"

RUN dotnet build "InventoryManagementSystem.csproj" \
    -c Release \
    -o /app/build \
    --no-restore

# ─────────────────────────────────────────────────────────────────────────────
# Stage 2: Publish — produce the self-contained publish folder
# ─────────────────────────────────────────────────────────────────────────────
FROM build AS publish

RUN dotnet publish "InventoryManagementSystem.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ─────────────────────────────────────────────────────────────────────────────
# Stage 3: Runtime — lean ASP.NET runtime image only
# ─────────────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Create a non-root user for security best-practice
RUN addgroup --system appgroup \
 && adduser  --system --ingroup appgroup appuser

COPY --from=publish /app/publish .

# Switch to non-root user
USER appuser

# Expose HTTP port — TLS is terminated at the Ingress / reverse-proxy layer
EXPOSE 8080

# Tell Kestrel to listen on 0.0.0.0:8080 (no HTTPS inside container)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "InventoryManagementSystem.dll"]
