namespace Prueba_Tecnica_Steven.Model
{
    public class Bill
    {
        public string billCode { get; set; }

        public double billTotal { get; set; }    
        public double billSubTotal { get; set; }
        public double billIva { get; set; }
        public double billRetention { get; set; }
        public DateTime billDate { get; set; }
        public string billStatus { get; set; }
        public bool billpay { get;set; }
        public DateTime billPaymetDate { get; set; }

    }
}
