using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {

        private readonly DataContext _context;

        public PokemonController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pokemon>>> Get()
        {
            return Ok(await _context.Pokemons.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pokemon>> Get(int id)
        {

            var pokemon = await _context.Pokemons.ToListAsync();

            var result = pokemon.Where(x => x.Id == id);

            //var pokemon = await _context.Pokemons.FindAsync(id);

            if (result == null)
                return BadRequest("Pokémon não encontrado");

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<Pokemon>>> AddPokemon([FromBody] Pokemon pkm)
        {
            _context.Pokemons.Add(pkm);
            await _context.SaveChangesAsync();
            return Ok(await _context.Pokemons.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<Pokemon>>> UpdatePokemon(Pokemon request)
        {
            var dbPokemon = await _context.Pokemons.FindAsync(request.Id);

            if (dbPokemon == null)
                return BadRequest("Pokémon não encontrado");

            dbPokemon.Name = request.Name;
            dbPokemon.Type = request.Type;
            dbPokemon.Region = request.Region;
            dbPokemon.Hp = request.Hp;
            dbPokemon.Attack = request.Attack;

            await _context.SaveChangesAsync();

            return Ok(await _context.Pokemons.ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Pokemon>>> DeletePokemon(int id)
        {
            var dbPokemon = await _context.Pokemons.FindAsync(id);

            if (dbPokemon == null)
                return BadRequest("Pokémon não encontrado");

            _context.Pokemons.Remove(dbPokemon);
            await _context.SaveChangesAsync();
            return Ok(await _context.Pokemons.ToListAsync());
        }

        //Busca Região
        [HttpGet("LinqRegion")]
        public async Task<ActionResult<List<Pokemon>>> GetPokeRegion([FromQuery] string filter, [FromQuery] string orderBy)
        {

            var pokemon = await _context.Pokemons.ToListAsync();

            var result = pokemon.Where(x => x.Region == filter);

            var filteredData = result;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filteredData = result
                    .Where(x => x.Region == filter).ToList();
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var propertyInfo = typeof(Pokemon).GetProperty(orderBy);
                filteredData = filteredData.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
            }

            if (result == null)
            {
                return Ok(new
                {
                    message = "Pokémon não encontrado"
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Deu certo!",
                Data = filteredData
            });
        }

        //----------------------------



        //Busca Tipo
        [HttpGet("LinqType")]
        public async Task<ActionResult<List<Pokemon>>> GetPokeType([FromQuery] string filter, [FromQuery] string orderBy)
        {
            var pokemon = await _context.Pokemons.ToListAsync();

            var result = pokemon.Where(x => x.Type.Contains(filter));

            var filteredData = result;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filteredData = result
                    .Where(x => x.Type.Contains(filter)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var propertyInfo = typeof(Pokemon).GetProperty(orderBy);
                filteredData = filteredData.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
            }

            if (result == null)
            {
                return Ok(new
                {
                    message = "Pokémon não encontrado"
                });
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Deu certo!",
                Data = filteredData
            });
        }



        //---------------------------


        [HttpGet("Battle_TesteAlfa")]
        public async Task<ActionResult<List<Pokemon>>> GetBattle([FromQuery] int id1, [FromQuery] int id2)
        {

            var poke1 = await _context.Pokemons.FindAsync(id1);
            var poke2 = await _context.Pokemons.FindAsync(id2);

            var Poke1Battlepoints = poke1.Hp + poke1.Attack;
            var Poke2Battlepoints = poke2.Hp + poke2.Attack;

            if (Poke1Battlepoints > Poke2Battlepoints)
            {
                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"Pokémon {poke1.Name} venceu!",
                    Data = poke1
                });
            }
            else if (Poke1Battlepoints < Poke2Battlepoints)
            {
                return Ok(new
                {
                    StatusCode = 200,
                    Message = $"Pokémon {poke2.Name} venceu!",
                    Data = poke2
                });
            }
            else
            {
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Empatou!",
                    Data = poke1 + "\n" + poke2
                });
            }
        }
    }
}
