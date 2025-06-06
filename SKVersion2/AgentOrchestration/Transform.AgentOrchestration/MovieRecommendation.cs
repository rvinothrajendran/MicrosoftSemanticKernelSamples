namespace Transform.AgentOrchestration;

/// <summary>
/// Represents a movie recommendation with details such as title, genre, description, language, and release year.
/// </summary>
public class MovieRecommendation
{
    /// <summary>
    /// Gets or sets the title of the recommended movie.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the genre of the recommended movie.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// Gets or sets the description of the recommended movie.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the language of the recommended movie.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the release year of the recommended movie.
    /// </summary>
    public string? ReleaseYear { get; set; }

    /// <summary>
    /// Returns a string representation of the movie recommendation.
    /// </summary>
    /// <returns>A formatted string containing the movie's details.</returns>
    public override string ToString()
    {
        return $" Title : {Title} \n Genre:({Genre}) \n Description:{Description} \n Language: {Language} \n Release Year: {ReleaseYear}";
    }
}