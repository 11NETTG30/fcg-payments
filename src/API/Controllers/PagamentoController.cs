using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Exceptions;
using FCG.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FCG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PagamentoController : ControllerBase
{
    private readonly IPagamentoService _appService;
    private readonly ILogger<PagamentoController> _logger;

    const string _erroInternoMsg = "Erro interno ao processar a requisição";

    public PagamentoController(
        IPagamentoService appService,
        ILogger<PagamentoController> logger)
    {
        _appService = appService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza o pagamento do pedido
    /// </summary>
    /// <param name="id">Id do pedido que será pago</param>
    /// <param name="request">Dados do pagamento do pedido</param>
    /// <returns>Confirmação do pagamento</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> Pagar([FromBody] PagamentoRequest request)
    {
        try
        {
            var pagamento = await _appService.ProcessarAsync(request);

            return CreatedAtAction(
                nameof(Pagar),
                new { id = pagamento.Id },
                ApiResponse<Guid>.Success(pagamento.Id, "Pedido pago com sucesso")
            );
        }
        catch (PagamentoRecusadoException ex)
        {
            return CreatedAtAction(
                    nameof(Pagar),
                    new { id = ex.PagamentoId },
                    ApiResponse<bool>.Failure($"O pagamento {ex.PagamentoId} foi recusado.")
                );
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao processar pagamento do pedido {PedidoId}", request.PedidoId);
            return BadRequest(ApiResponse<object>.Failure(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pagamento do pedido {PedidoId}", request.PedidoId);
            return StatusCode(500, ApiResponse<object>.Failure(
                _erroInternoMsg
            ));
        }
    }
    

    /// <summary>
    /// Obtém um pagamento por ID
    /// </summary>
    /// <param name="id">Id do pagamento buscado</param>
    /// <returns>Dados do pagamento</returns>
    /// <response code="200">Pagamento obtido com sucesso</response>
    /// <response code="404">Pagamento não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}", Name = "ObterPagamento")]
    [ProducesResponseType(typeof(ApiResponse<PagamentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagamentoDto>>> Obter(Guid id)
    {
        try
        {
            var pagamento = await _appService.ObterPagamentoAsync(id);

            if (pagamento == null)
            {
                return NotFound(ApiResponse<object>.Failure(
                    $"Pagamento {id} não encontrado"
                ));
            }

            return Ok(ApiResponse<PagamentoDto>.Success(pagamento));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao buscar pagamento {PagamentoId}", id);
            return BadRequest(ApiResponse<object>.Failure(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pagamento {PagamentoId}", id);

            return StatusCode(500, ApiResponse<object>.Failure(
                _erroInternoMsg
            ));
        }
    }

    /// <summary>
    /// Lista pagamentos do usuário
    /// </summary>
    /// /// <param name="usuarioId">Id do usuário</param>
    /// <returns>Lista de pagamentos do usuário</returns>
    [HttpGet("usuario/{usuarioId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<PagamentoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PagamentoDto>>>> ListarPorUsuario(
        Guid usuarioId)
    {
        try
        {
            var pagamentos = await _appService.ListarPagamentosPorUsuarioAsync(usuarioId);
            return Ok(ApiResponse<List<PagamentoDto>>.Success(pagamentos));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao listar pagamentos do usuário {UsuarioId}", usuarioId);
            return BadRequest(ApiResponse<object>.Failure(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pagamentos do usuário {UsuarioId}", usuarioId);
            return StatusCode(500, ApiResponse<object>.Failure(
                _erroInternoMsg
            ));
        }
    }
}
