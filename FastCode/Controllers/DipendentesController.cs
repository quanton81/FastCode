using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FastCode.Data;
using FastCode.Models;
using FastCode.ViewModels;

namespace FastCode.Controllers
{
    public class DipendentesController : Controller
    {
        private readonly FastCodeContext _context;

        public DipendentesController(FastCodeContext context)
        {
            _context = context;
        }

        private async Task<List<DipendenteView>> GetAllDipendenti()
        {
            var dipendenti = from dipendente in _context.Dipendente
                   join manager in _context.Dipendente on dipendente.Manager equals manager.Id into mann
                   from subman in mann.DefaultIfEmpty()
                   select new DipendenteView
                   {
                       Id = dipendente.Id,
                       Nome = dipendente.Nome,
                       Matricola = dipendente.Matricola,
                       Manager = subman.Nome ?? null
                   };

            return await dipendenti.ToListAsync();
        }

        // GET: Dipendentes
        public async Task<IActionResult> Index(string matricola)
        {

            if(string.IsNullOrEmpty(matricola))
            {
                return View(await GetAllDipendenti());
            }
            else
            {
                string cteQuery = @"
with cte as
(
  select Id, Nome, Matricola, Manager
  from   Dipendente
  where  Matricola = {0}

  union all

  select t.Id, t.Nome, t.Matricola, t.Manager
  from   cte c                                   
  inner join Dipendente t 
  on c.Manager = t.Id
)
select *
from cte where Manager is null";

                var cteDipendenti = _context.Dipendente.FromSqlRaw(cteQuery, matricola).AsEnumerable();

                if(cteDipendenti.Any())
                {
                    return View(cteDipendenti.Select(dipendente => new DipendenteView
                    {
                        Id = dipendente.Id,
                        Nome = dipendente.Nome,
                        Matricola = dipendente.Matricola,
                        Manager = null
                    }).ToList());
                }

                return View();
            }
        }

        // GET: Dipendentes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dipendenteTrova = from dipendente in _context.Dipendente
                join manager in _context.Dipendente on dipendente.Manager equals manager.Id into mann
                from subman in mann.DefaultIfEmpty()
                where dipendente.Id == id
                select new DipendenteView
                {
                    Id = dipendente.Id,
                    Nome = dipendente.Nome,
                    Matricola = dipendente.Matricola,
                    Manager = subman.Nome ?? null
                };

            var dipendenteTrovato = await dipendenteTrova.FirstOrDefaultAsync();

            if (dipendenteTrovato == null)
            {
                return NotFound();
            }

            return View(dipendenteTrovato);
        }

        // GET: Dipendentes/Create
        public async Task<IActionResult> Create()
        {
            var options = await _context.Dipendente.Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                }).ToListAsync();

            options.Add(new SelectListItem
            {
                Value = "",
                Text = "Vuoto"
            });

            var data = new Tuple<Dipendente, List<SelectListItem>> (new Dipendente(), options);

            return View(data);
        }

        // POST: Dipendentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Matricola,Manager")] Dipendente dipendente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dipendente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dipendente);
        }

        // GET: Dipendentes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dipendente = await _context.Dipendente.FindAsync(id);
            if (dipendente == null)
            {
                return NotFound();
            }

            var options = await _context.Dipendente.Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                }).ToListAsync();

            options.Add(new SelectListItem
            {
                Value = "",
                Text = "Vuoto"
            });

            options = options.Select(
                l => new SelectListItem { 
                    Selected = (l.Value == id.ToString() || l.Value == ""), 
                    Text = l.Text, 
                    Value = l.Value }
            ).ToList();

            var data = new Tuple<Dipendente, List<SelectListItem>>(dipendente, options);

            return View(data);
        }

        // POST: Dipendentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Matricola,Manager")] Dipendente dipendente)
        {
            if (id != dipendente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dipendente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DipendenteExists(dipendente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var options = await _context.Dipendente.Select(a =>
                new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                }).ToListAsync();

            options = options.Select(
                l => new SelectListItem { 
                    Selected = (l.Value == id.ToString()), 
                    Text = l.Text, 
                    Value = l.Value }
            ).ToList();

            var data = new Tuple<Dipendente, List<SelectListItem>>(dipendente, options);

            return View(data);
        }

        // GET: Dipendentes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dipendente = await _context.Dipendente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dipendente == null)
            {
                return NotFound();
            }

            return View(dipendente);
        }

        // POST: Dipendentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dipendente = await _context.Dipendente.FindAsync(id);
            _context.Dipendente.Remove(dipendente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DipendenteExists(int id)
        {
            return _context.Dipendente.Any(e => e.Id == id);
        }
    }
}
