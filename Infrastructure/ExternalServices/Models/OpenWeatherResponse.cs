using System.Text.Json.Serialization;

namespace Infrastructure.ExternalServices.Models
{
    public record OpenWeatherResponse(
        [property: JsonPropertyName("coord")]
        Coord Coord,

        [property: JsonPropertyName("main")]
        Main Main,

        [property: JsonPropertyName("weather")]
        List<Weather> Weather,

        [property: JsonPropertyName("sys")]
        Sys Sys,

        [property: JsonPropertyName("name")]
        string Name
    );

    public record Coord(
        [property: JsonPropertyName("lon")]
        decimal Lon,

        [property: JsonPropertyName("lat")]
        decimal Lat
    );

    public record Weather(
        [property: JsonPropertyName("description")]
        string Description
    );

    public record Main(
        [property: JsonPropertyName("temp")]
        double Temp,

        [property: JsonPropertyName("temp_min")]
        double TempMin,

        [property: JsonPropertyName("temp_max")]
        double TempMax
    );

    public record Sys(
        [property: JsonPropertyName("country")]
        string Country
    );
}