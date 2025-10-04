using System;
using Domain.Tests;

namespace DDD.Domain
{
    public class Test
    {
        public static void Main()
        {
            // Вызов тестов для Property
            // PropertyTest.RunPropertyTests();
            
            // Вызов тестов для Client
            ClientTest.RunClientTests();
        }
    }
}