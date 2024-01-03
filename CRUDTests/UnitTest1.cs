namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //attach
            int input1 = 7, input2 = 8, expected = 15;
            
            //Act
            dummyClass dc= new dummyClass();
            int sum=dc.add(input1,input2);

            //Assert
            Assert.Equal(expected, sum);

        }
    }
}