// TransactionManagementService.API/Controllers/TransactionsController.cs
using Microsoft.AspNetCore.Mvc;
using TransactionManagementService.Application.DTOs;
using TransactionManagementService.Application.Services;

namespace TransactionManagementService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Obtém todas as transações
        /// </summary>
        /// <returns>Lista de transações</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        /// <summary>
        /// Obtém uma transação específica por ID
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <returns>Transação encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> GetTransaction(string id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            
            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            return Ok(transaction);
        }

        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        /// <param name="createTransactionDto">Dados da transação</param>
        /// <returns>Transação criada</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionDto createTransactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(createTransactionDto);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}