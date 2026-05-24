namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class InventoryTransactionLineSerial : Aggregate
{
    public Guid TransactionLineRef { get; private set; }
    public Guid? SerialRef { get; private set; }
    public string SerialNo { get; private set; } = string.Empty;

    private InventoryTransactionLineSerial()
    {
    }

    internal static InventoryTransactionLineSerial Create(Guid transactionLineRef, Guid? serialRef, string serialNo)
    {
        if (transactionLineRef == Guid.Empty)
            throw new ArgumentException("Transaction line reference is required.", nameof(transactionLineRef));

        if (string.IsNullOrWhiteSpace(serialNo))
            throw new ArgumentException("Serial number is required.", nameof(serialNo));

        return new InventoryTransactionLineSerial
        {
            TransactionLineRef = transactionLineRef,
            SerialRef = serialRef,
            SerialNo = serialNo.Trim()
        };
    }
}
