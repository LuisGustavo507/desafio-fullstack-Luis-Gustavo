namespace Domain.Exceptions
{
    public class NegocioException : DomainException
    {
        public NegocioException(string pMensagem) : base(pMensagem)
        {
        }
    }
}