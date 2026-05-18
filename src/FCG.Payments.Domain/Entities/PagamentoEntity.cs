using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities
{
    public class PagamentoEntity : Entity, IAggregateRoot
    {
        public Guid PedidoId { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid JogoId { get; private set; }
        public decimal Valor { get; private set; }
        public PagamentoStatus Status { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataPagamento { get; private set; }

        public PagamentoEntity(Guid usuarioId, Guid jogoId, decimal valor)
        {
            ValidarValor(valor);

            PedidoId = GerarPedidoId(usuarioId, jogoId);
            UsuarioId = usuarioId;
            JogoId = jogoId;
            DataCriacao = DateTime.UtcNow;
            Status = PagamentoStatus.Created;
            Valor = valor;
        }

        private static void ValidarValor(decimal valor)
        {
            if (valor <= 0)
                throw new DomainException("Valor do pagamento deve ser maior que zero");
        }

        #region Alteração de Status

        public void AprovarPagamento()
        {
            ValidarPodePagar();

            DataPagamento = DateTime.UtcNow;
            Status = PagamentoStatus.Approved;
        }

        public void RecusarPagamento()
        {
            ValidaSeStatusDiferenteCriado("Pagamento não pode ser recusado neste estado.");

            DataPagamento = DateTime.UtcNow;
            Status = PagamentoStatus.Rejected;
        }

        public void Cancelar()
        {
            ChecaPedidoJaPago();

            Status = PagamentoStatus.Cancelled;
        }

        #endregion

        #region Validacoes
        private void ChecaPedidoJaPago()
        {
            if (Status == PagamentoStatus.Approved)
                throw new PagamentoJaPagoException();
        }

        private void ValidaSeStatusDiferenteCriado(string msg)
        {
            if (Status != PagamentoStatus.Created)
                throw new DomainException(msg);
        }

        private void ValidarPodePagar()
        {
            if (Status == PagamentoStatus.Cancelled)
                throw new PagamentoCanceladoException();

            ChecaPedidoJaPago();
            ValidaSeStatusDiferenteCriado("Pagamento não pode ser aprovado neste estado.");
        }
        #endregion

        public static Guid GerarPedidoId(Guid usuarioId, Guid jogoId)
        {
            var input = $"{usuarioId}:{jogoId}";

            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return new Guid(hash);
        }

        public bool EstaPago() => Status == PagamentoStatus.Approved;

        public bool PodeSerAlterado() => Status == PagamentoStatus.Created;
    }
}
