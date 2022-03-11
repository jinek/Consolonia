namespace Example.Views
{
    public class TheItem
    {
        public static readonly string[] Titles =
        {
            "The 400 Blows", "La Haine", "The Godfather", "The Godfather: Part II", "Man Bites Dog", "The Departed",
            "Umberto D.", "White Heat", "Eddie Murphy: Raw", "All Quiet on the Western Front"
        };

        public static readonly string[] Genres = { "Comedy", "Horror", "Action", "Family", "Cartoons" };
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsListed { get; set; }
        public string Genre { get; set; }
    }
}