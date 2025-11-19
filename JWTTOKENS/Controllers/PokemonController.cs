using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTTOKENS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PokemonController : ControllerBase
    {
        private static readonly List<Models.PokemonModel> _pokemons = new()
        {
            new Models.PokemonModel { Id = 1, Name = "Bulbasaur", Type = "Grass/Poison", Level = 5, Description = "A strange seed was planted on its back at birth. The plant sprouts and grows with this Pokémon." },
            new Models.PokemonModel { Id = 2, Name = "Charmander", Type = "Fire", Level = 5, Description = "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail." },
            new Models.PokemonModel { Id = 3, Name = "Squirtle", Type = "Water", Level = 5, Description = "After birth, its back swells and hardens into a shell. Powerfully sprays foam from its mouth." }
        };

        [HttpGet]
        public IActionResult GetPokemons()
        {
            return Ok(_pokemons);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetPokemon(int id)
        {
            var pokemon = _pokemons.FirstOrDefault(p => p.Id == id);
            if (pokemon is null) return NotFound(new { Message = $"Pokemon con id {id} no encontrado." });
            return Ok(pokemon);
        }
    }
}
