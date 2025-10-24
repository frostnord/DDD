namespace Domain.Tests
{
    public class Test
    {
        public static void Main()
        {
            // Вызов тестов для Property
            PropertyTest.RunPropertyTests();
            
            // Вызов тестов для Seller
            SellerTest.RunSellerTests();
        }
    }
}