namespace IVPlay
{
    /// <summary>
    /// Determine what kind of art to draw
    /// </summary>
    public enum ArtType
    {
        snap = 0,
        flyer = 1,
        //cabinet = 2,
    }
    /// <summary>
    /// Determine how our art should be drawn.
    /// </summary>
    public enum ArtDisplayMode
    {
        normal = 0,
        vertical = 1,
        superlarge = 2,
    }
    /// <summary>
    /// What to search by
    /// </summary>
    public enum SearchType
    {
        Name,
        Description,
        Driver,
        Year
    }
    /// <summary>
    /// What text to draw
    /// </summary>
    public enum DisplayModeEnum
    {
        Description,
        DescriptionAndYear,
        DescriptionAndManufacturer,
        DescriptionYearAndManufacturer
    }

    public enum FavoritesMode
    {
        Games = 0,
        FavoritesAndGames = 1,
        Favorites = 2,
    }

    public enum InfoScrollMode
    {
        Line,
        Page,
        All
    }
}