using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Tournament
{
    public class AssignJudgesDTO
    {

        [Required(ErrorMessage = "Debes seleccionar jueces para asignar al torneo.")]
        public List<int> JudgeIds { get; set; }
    }
}
