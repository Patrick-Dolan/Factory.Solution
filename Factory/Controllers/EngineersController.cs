using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Factory.Models;
using System.Linq;

namespace Factory.Controllers
{
  public class EngineersController : Controller
  {
    private readonly FactoryContext _db;
    public EngineersController(FactoryContext db)
    {
      _db = db;
    }
    public ActionResult Index()
    {
      return View(_db.Engineers.ToList());
    }

    public ActionResult Create()
    {
      ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Name");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Engineer engineer, int MachineId)
    {
      _db.Engineers.Add(engineer);
      _db.SaveChanges();
      if(MachineId != 0)
      {
        _db.EngineerMachines.Add(new EngineerMachine() { EngineerId = engineer.EngineerId, MachineId = MachineId});
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Engineer engineer = _db.Engineers
        .Include(engineer => engineer.JoinEntities)
        .ThenInclude(join => join.Machine)
        .FirstOrDefault(record => record.EngineerId == id);
      return View(engineer);
    }

    public ActionResult Edit(int id)
    {
      Engineer engineer = _db.Engineers.FirstOrDefault(record => record.EngineerId == id);
      ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Name");
      return View(engineer);
    }

    [HttpPost]
    public ActionResult Edit(Engineer engineer, int MachineId)
    {
      if(MachineId != 0)
      {
        _db.EngineerMachines.Add(new EngineerMachine{ EngineerId = engineer.EngineerId, MachineId = MachineId});
      }
      _db.Entry(engineer).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = engineer.EngineerId});
    }

    public ActionResult Delete(int id)
    {
      Engineer engineer = _db.Engineers.FirstOrDefault(record => record.EngineerId == id);
      return View(engineer);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Engineer engineer = _db.Engineers.FirstOrDefault(record => record.EngineerId == id);
      _db.Engineers.Remove(engineer);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddMachine(int id)
    {
      Engineer engineer = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
      ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Name");
      return View(engineer);
    }

    [HttpPost]
    public ActionResult AddMachine(Engineer engineer, int MachineId)
    {
      if (MachineId != 0)
      {
        _db.EngineerMachines.Add(new EngineerMachine() {EngineerId = engineer.EngineerId, MachineId = MachineId});
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = engineer.EngineerId});
    }
    
    [HttpPost]
    public ActionResult DeleteMachine(int joinId)
    {
      var JoinEntry = _db.EngineerMachines.FirstOrDefault(entry => entry.EngineerMachineId == joinId);
      _db.EngineerMachines.Remove(JoinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = JoinEntry.EngineerId});
    }
  }
}