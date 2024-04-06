using API._Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModels.System;

namespace API.Controllers.System;

public class CommandsController : BaseController
{
    private readonly IRepositoryAccessor _repositoryAccessor;
    public CommandsController(IRepositoryAccessor repositoryAccessor)
    {
        _repositoryAccessor = repositoryAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetCommands()
    {
        return Ok(await _repositoryAccessor.Commands.FindAll(true).Select(x => new CommandVm()
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync());
    }
}
