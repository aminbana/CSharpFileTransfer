using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using System.Runtime.CompilerServices;


namespace MMWSoftware
{
    public class MQSocket
    {
        private RequestSocket MQ_Sender;
        private ResponseSocket MQ_Receiver;
        
        public MQSocket (int port_number , Boolean isFirstToReceive, String peer_ip)
        {
            if (isFirstToReceive)
            {
                MQ_Receiver = new ResponseSocket();
                MQ_Receiver.Bind("tcp://*:" + (port_number + 1).ToString()); //Receiver means it should start with recieving
                var msg = recv_string();
                Console.WriteLine("MQ_Sender received {0}", msg);
               

                MQ_Sender = new RequestSocket();
                MQ_Sender.Connect($"tcp://{peer_ip}:" + port_number.ToString());
                send_string("sample");

                
            } else {
                MQ_Sender = new RequestSocket();
                MQ_Sender.Connect($"tcp://{peer_ip}:" + (port_number + 1).ToString());
                var msg = send_string("Init");
                Console.WriteLine("MQ_Sender received {0}", msg);
                

                MQ_Receiver = new ResponseSocket();
                MQ_Receiver.Bind("tcp://*:" + port_number.ToString()); //Receiver means it should start with recieving

                msg = recv_string();
                Console.WriteLine("MQ_Receiver received {0}", msg);

            }

        }

        public MQSocket(int port_number) : this(port_number, false, "127.0.0.1") { }

        public String send_string(String message)
        {
            MQ_Sender.SendFrame(message);
            return MQ_Sender.ReceiveFrameString();
        }


        public String send_file(String file_path)
        {
            byte[] buffer = File.ReadAllBytes(file_path);
            var filename = Path.GetFileName(file_path);
            var msg = this.send_string(filename);
            return this.send_bytes(buffer);


        }

        public String recv_file(String base_path)
        {
            var filename = this.recv_string();
            var buffer = this.recv_bytes();
            File.WriteAllBytes(base_path + filename, buffer);
            return filename;
        }

        public String recv_string()
        {
            var msg = this.MQ_Receiver.ReceiveFrameString();
            MQ_Receiver.SendFrame("Done");
            return msg;
        }

        public String send_bytes(byte[] buffer)
        {
            MQ_Sender.SendFrame(buffer);
            return MQ_Sender.ReceiveFrameString();
        }

        public byte[] recv_bytes()
        {
            var bytes = MQ_Receiver.ReceiveFrameBytes();
            MQ_Receiver.SendFrame("Done");
            return bytes;
        }

        public T[,,,] receive_4DArray<T>()
        {
            String dimSizes = recv_string();

            int dim0 = int.Parse(dimSizes.Split(',')[0]);
            int dim1 = int.Parse(dimSizes.Split(',')[1]);
            int dim2 = int.Parse(dimSizes.Split(',')[2]);
            int dim3 = int.Parse(dimSizes.Split(',')[3]);

            byte[] buffer = recv_bytes();
            T[] double_buffer = new T[buffer.Length / Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(buffer, 0, double_buffer, 0, buffer.Length);

            T[,,,] array = new T[dim0, dim1, dim2, dim3];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                for (int c = 0; c < dim1; c++)
                {
                    for (int y = 0; y < dim2; y++)
                    {
                        for (int x = 0; x < dim3; x++)
                        {

                            array[v, c, y, x] = double_buffer[i];
                            i++;
                        }
                    }
                }

            }

            return array;
        }

        public T[,,] receive_3DArray<T>()
        {
            String dimSizes = recv_string();

            int dim0 = int.Parse(dimSizes.Split(',')[0]);
            int dim1 = int.Parse(dimSizes.Split(',')[1]);
            int dim2 = int.Parse(dimSizes.Split(',')[2]);

            byte[] buffer = recv_bytes();
            T[] double_buffer = new T[buffer.Length / Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(buffer, 0, double_buffer, 0, buffer.Length);

            T[,,] array = new T[dim0, dim1, dim2];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                for (int c = 0; c < dim1; c++)
                {
                    for (int y = 0; y < dim2; y++)
                    {
                        array[v, c, y] = double_buffer[i];
                        i++;
                    }
                }

            }

            return array;
        }

        public T[,] receive_2DArray<T>()
        {
            String dimSizes = recv_string();

            int dim0 = int.Parse(dimSizes.Split(',')[0]);
            int dim1 = int.Parse(dimSizes.Split(',')[1]);

            byte[] buffer = recv_bytes();
            T[] double_buffer = new T[buffer.Length / Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(buffer, 0, double_buffer, 0, buffer.Length);

            T[,] array = new T[dim0, dim1];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                for (int c = 0; c < dim1; c++)
                {
                    array[v, c] = double_buffer[i];
                    i++;
                }

            }

            return array;
        }

        public T[] receive_1DArray<T>()
        {
            String dimSizes = recv_string();

            int dim0 = int.Parse(dimSizes.Split(',')[0]);

            byte[] buffer = recv_bytes();
            T[] double_buffer = new T[buffer.Length / Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(buffer, 0, double_buffer, 0, buffer.Length);

            T[] array = new T[dim0];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                array[v] = double_buffer[i];
                i++;
            }

            return array;
        }

        public String send_4DArray<T>(T[,,,] array)
        {

            int dim0 = array.GetLength(0); ;
            int dim1 = array.GetLength(1); ;
            int dim2 = array.GetLength(2); ;
            int dim3 = array.GetLength(3); ;

            String dimSizes = dim0.ToString() + "," + dim1.ToString() + "," + dim2.ToString() + "," + dim3.ToString();
            send_string(dimSizes);

            int array_length = dim0 * dim1 * dim2 * dim3;

            T[] flatten_array = new T[array_length];

            //double[,,,] array = new double[dim0, dim1, dim2, dim3];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                for (int c = 0; c < dim1; c++)
                {
                    for (int y = 0; y < dim2; y++)
                    {
                        for (int x = 0; x < dim3; x++)
                        {

                            flatten_array[i] = array[v, c, y, x];
                            i++;
                        }
                    }
                }

            }
            byte[] buffer = new byte[array_length * Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(flatten_array, 0, buffer, 0, buffer.Length);
            return send_bytes(buffer);
        }

        public String send_3DArray<T>(T[,,] array)
        {

            int dim0 = array.GetLength(0);
            int dim1 = array.GetLength(1);
            int dim2 = array.GetLength(2);

            String dimSizes = dim0.ToString() + "," + dim1.ToString() + "," + dim2.ToString();
            send_string(dimSizes);

            int array_length = dim0 * dim1 * dim2;

            T[] flatten_array = new T[array_length];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                for (int c = 0; c < dim1; c++)
                {
                    for (int y = 0; y < dim2; y++)
                    {

                            flatten_array[i] = array[v, c, y];
                            i++;
                    }
                }

            }
            byte[] buffer = new byte[array_length * Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(flatten_array, 0, buffer, 0, buffer.Length);
            return send_bytes(buffer);
        }

        public String send_2DArray<T>(T[,] array)
        {

            int dim0 = array.GetLength(0);
            int dim1 = array.GetLength(1);

            String dimSizes = dim0.ToString() + "," + dim1.ToString();
            send_string(dimSizes);

            int array_length = dim0 * dim1;

            T[] flatten_array = new T[array_length];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                for (int c = 0; c < dim1; c++)
                {
                        flatten_array[i] = array[v, c];
                        i++;
                    
                }

            }
            byte[] buffer = new byte[array_length * Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(flatten_array, 0, buffer, 0, buffer.Length);
            return send_bytes(buffer);
        }
        public String send_1DArray<T>(T[] array)
        {

            int dim0 = array.GetLength(0);

            String dimSizes = dim0.ToString();
            send_string(dimSizes);

            int array_length = dim0;

            T[] flatten_array = new T[array_length];

            int i = 0;
            for (int v = 0; v < dim0; v++)
            {
                    flatten_array[i] = array[v];
                    i++;
            }
            byte[] buffer = new byte[array_length * Unsafe.SizeOf<T>()];
            Buffer.BlockCopy(flatten_array, 0, buffer, 0, buffer.Length);
            return send_bytes(buffer);
        }

    }


}
