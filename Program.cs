using MPI;

using var mpi = new MPI.Environment(ref args);

int rank = Communicator.world.Rank;
int size = Communicator.world.Size;

if (size != 16)
    throw new Exception();

var dishes = new List<string>() { "Котлетки с макарошками", "Котлетки с пюрешкой", "Супчик", "Борщ", "Салатик", "Пирожок с картошкой" };
var rnd = new Random();

if (rank == 0)
{
    for (int i = 1; i < size; i++)
    {
        Communicator.world.Send(i, i, 0);
        Console.WriteLine($"Повар спросил у студента {i} что он хочет");

        var request = Communicator.world.Receive<string>(i, 0);
        Console.WriteLine($"Повар готовит \"{request}\" для студента {i}");
        Thread.Sleep(100);
        Communicator.world.Send(request, i, 0);
        Console.WriteLine($"Повар отдал заказ \"{request}\" для студента {i}");

        request = Communicator.world.Receive<string>(i, 0);
        Console.WriteLine($"Повар еще раз готовит для студента {i} блюдо \"{request}\" ");
        Thread.Sleep(100);
        Console.WriteLine($"Повар отдал заказ \"{request}\" для студента {i}");
        Communicator.world.Send(request, i, 0);
    }
} 
else
{
    int order;

    do
    {
        order = Communicator.world.Receive<int>(0, 0);
    } while (order != rank);

    var dish = dishes[rnd.Next(0, dishes.Count)]; 
    Communicator.world.Send(dish, 0, 0);
    Console.WriteLine($"Голодный студент {rank} заказал \"{dish}\"");
    var requestedDish = Communicator.world.Receive<string>(0, 0);
    Console.WriteLine($"Студент {rank} получил \"{requestedDish}\"");

    dish = dishes[rnd.Next(0, dishes.Count)];
    Communicator.world.Send(dish, 0, 0);
    Console.WriteLine($"Голодный студент {rank} заказал \"{dish}\"");
    requestedDish = Communicator.world.Receive<string>(0, 0);
    Console.WriteLine($"Студент {rank} получил \"{requestedDish}\"");
}