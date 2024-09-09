using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camposol.Models
{
    /// <summary>
    /// Settings class
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Max duration recording
        /// </summary>
        public int MaxDurationInMinutes { get; set; } = 2;
    }
}
