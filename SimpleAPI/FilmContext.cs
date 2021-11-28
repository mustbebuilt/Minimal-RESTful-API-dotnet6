using Microsoft.EntityFrameworkCore;

namespace SimpleAPI
{
    public class FilmContext : DbContext
    {
        public FilmContext(DbContextOptions<FilmContext> options) : base(options)
        {

        }

        public DbSet<Film> Films { get; set; }
    }

    public class Film
    {
 
        public int FilmID { get; set; }

        public string FilmTitle { get; set; }

        public string FilmCertificate { get; set; }

        public string FilmDescription { get; set; }

        public string FilmImage { get; set; }

        public decimal FilmPrice { get; set; }

        public int Stars { get; set; }

        public DateTime ReleaseDate { get; set; }

    }
}