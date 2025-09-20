using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Infrastructure.DTOs;
[DataContract]
public class RetrieveNutritionDataRequestDto
{
    [JsonPropertyName("query")]
    public string Query { get; set; }
}