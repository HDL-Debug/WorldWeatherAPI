using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WorldWeatherAPI.Data.Models
{
    [Table("Cities")]
    [Index(nameof(Name))]
    [Index(nameof(Temp))]
    [Index(nameof(Perci))]
    public class City
    {
        #region Properties
        /// <summary>
        /// A collection of all the cities related to this country.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// City name (in UTF8 format)
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Average Temperature
        /// </summary>
        [Column(TypeName = "decimal(7,4)")]
        public decimal Temp { get; set; }
        /// <summary>
        /// Average Percipation
        /// </summary>
        [Column(TypeName = "decimal(7,4)")]
        public decimal Perci { get; set; }
        /// <summary>
        /// Country Id (foreign key)
        /// </summary>
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        #endregion

        #region Navigation Properties
        /// The country related to this city.
        public Country? Country { get; set; } = null!;
        #endregion
    }
}
