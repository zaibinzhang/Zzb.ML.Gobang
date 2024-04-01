using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.KerasApi;

var inputs = keras.Input(shape: (15, 15, 1), name: "棋盘qipan");
var x1 = keras.layers.Conv2D(32, 3, activation: "relu", padding: "same").Apply(inputs);
var x2 = keras.layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x1);
var x3 = keras.layers.Conv2D(128, 3, activation: "relu", padding: "same").Apply(x2);
//var x4 = keras.layers.Conv2D(256, 3, activation: "relu", padding: "same").Apply(x3);
//var x5 = keras.layers.Conv2D(128, 3, activation: "relu", padding: "same").Apply(x4);
var x6 = keras.layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x3);
var x7 = keras.layers.Conv2D(32, 3, activation: "relu", padding: "same").Apply(x6);
var x8 = keras.layers.Conv2D(16, 3, activation: "relu", padding: "same").Apply(x7);
var x9 = keras.layers.Conv2D(4, 3, activation: "relu", padding: "same").Apply(x8);
var x10 = keras.layers.Flatten().Apply(x9);
var x11 = keras.layers.Dense(225, activation: "softmax").Apply(x10);

var model = keras.Model(inputs, x11, name: "gomoku_cnn");

model.compile(keras.optimizers.Adam(0.0001f), keras.losses.CategoricalCrossentropy(), new[] { "accuracy" });


{
    var l = 10000;

    // 假设你的棋盘数据是一个15x15的二维数组
    float[,,] boardData = new float[l, 15, 15];
    // 假设你的标签数据是一个长度为225的数组
    float[,] labelData = new float[l, 225];

    var random = new Random();

    for (int i = 0; i < l; i++)
    {
        var x = random.Next(15);
        var y = random.Next(15);
        boardData[i, x, y] = 1;
        labelData[i, x * 15 + y] = 1;
    }

    // 将棋盘数据转换为模型所需的格式
    Tensor boardTensor = new Tensor(boardData, new Shape(l, 15, 15, 1)); // 加入批次维度和颜色通道

    // 将标签数据转换为张量
    Tensor labelTensor = new Tensor(labelData, new Shape(l, 225)); // 加入批次维度

    model.fit(boardTensor.numpy(), labelTensor.numpy(), epochs: 10);
}

{
    float[,] newBoardData = new float[15, 15];
    newBoardData[1, 1] = 1;


    // 将数据转换为Tensor
    Tensor newBoardTensor = new Tensor(newBoardData, new Shape(1, 15, 15, 1));

    // 使用模型进行预测
    var prediction = model.predict(newBoardTensor.numpy());

    var nArray = prediction.numpy();

    // 输出预测结果
    Console.WriteLine(prediction.ToString());
}


Console.WriteLine("完成");