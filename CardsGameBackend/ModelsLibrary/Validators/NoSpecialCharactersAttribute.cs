using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelsLibrary.Validators
{
    public class NoSpecialCharactersAttribute : ValidationAttribute
    {
        private static readonly Regex _regex = new Regex("^[a-zA-Z0-9]*$", RegexOptions.Compiled);

        public NoSpecialCharactersAttribute() : base("El nombre de usuario solo puede contener letras y números.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true; // Si el valor es null, no se valida (esto lo manejará el Required)

            string username = value.ToString();
            return _regex.IsMatch(username); // Verifica si el username solo tiene letras y números
        }
    }
}
