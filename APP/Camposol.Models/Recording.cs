using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camposol.Models
{
    public class Recording : Bindableitem
    { 
        private bool sended = false;

        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } 

        /// <summary>
        /// Audio path
        /// </summary>
        public string AudioPath { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Lote
        /// </summary>
        public string Lote { get; set; }

        /// <summary>
        /// Plant
        /// </summary>
        public string Plant { get; set; }

        /// <summary>
        /// Row
        /// </summary>
        public string Row { get; set; }

        /// <summary>
        /// Sended
        /// </summary>
        public bool Sended
        {
            get => this.sended;
            set
            {
                this.sended = value;
                OnPropertyChanged(nameof(Sended));
            }
        }
    }
}
