#!/bin/sh
# Replace the API base URL in the Blazor appsettings.json at container startup.
# This allows the same image to work on any host without rebuilding.
# Falls back to the build-time default if API_BASE_URL is not set.

if [ -n "$API_BASE_URL" ]; then
  # Use a temp file so sed works on alpine
  sed "s|\"ApiBaseUrl\":.*|\"ApiBaseUrl\": \"${API_BASE_URL}\"|" \
    /usr/share/nginx/html/appsettings.json > /tmp/appsettings.json
  mv /tmp/appsettings.json /usr/share/nginx/html/appsettings.json
fi

exec "$@"
