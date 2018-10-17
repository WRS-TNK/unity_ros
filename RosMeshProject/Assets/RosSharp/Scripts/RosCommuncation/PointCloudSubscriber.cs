﻿using System;
using System.Collections;
using System.Collections.Generic;
using RosSharp.RosBridgeClient.Messages.Sensor;
using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class PointCloudSubscriber : Subscriber<Messages.Sensor.PointCloud2>
    {
        private byte[] byteArray;
        private bool isMessageReceived = false;
        private int size;
        private Mesh mesh;

        public Transform test_point;

        public GameObject floorObject;
        static public Vector3 floor_posi;

        private Vector3[] pcl;

        protected override void Start()
        {
            base.Start();

        }

        public void Update()
        {
            
            if (isMessageReceived)
            {
                //CreateMesh();
                test_point.position = new Vector3(pcl[0].x, pcl[0].y, pcl[0].z);
                isMessageReceived = false;
            }
        }

        protected override void ReceiveMessage(PointCloud2 message)
        {
            //throw new NotImplementedException();
            //size = message.data.GetLength(0);
            //int i=0;

            /*
            foreach (byte temp in message.data)
            {
                byteArray[i] = temp;
                i++;
            }
            Debug.Log(size);*/
            isMessageReceived = true;
            //byte width = (byte)message.width;
            //byte height = (byte)message.height;
            //byte row_step = (byte)message.row_step;
            //byte point_step = (byte)message.point_step;

            int width = message.width;
            int height = message.height;
            int row_step = message.row_step;
            int point_step = message.point_step;

            Debug.Log("width" + message.width);
            Debug.Log("height" + message.height);
            Debug.Log("row_step" + message.row_step);
            Debug.Log("point_step" + message.point_step);
            
            size = width*height;
            Debug.Log(size);
            //BitConverter.ToSingle(test, 0);

            pcl = new Vector3[size];
            float min_x = 0.0f;
            float min_y = 0.0f;
            float min_z = 0.0f;
            float max_x = 0.0f;
            float max_y = 0.0f;
            float max_z = 0.0f;

            for (int n = 0; n < size; n++)
            {
                int x_posi = n * message.point_step + message.fields[0].offset;
                int y_posi = n * message.point_step + message.fields[1].offset;
                int z_posi = n * message.point_step + message.fields[2].offset;

                float x = BitConverter.ToSingle(message.data, x_posi);
                float y = BitConverter.ToSingle(message.data, y_posi);
                float z = BitConverter.ToSingle(message.data, z_posi);

                //Debug.Log(x);
                //Debug.Log(y);
                //Debug.Log(z);

                min_x = ComFloat(x, min_x, "min");
                min_y = ComFloat(y, min_y, "min");
                min_z = ComFloat(z, min_z, "min");
                max_x = ComFloat(x, max_x, "max");
                max_y = ComFloat(y, max_y, "max");
                max_z = ComFloat(z, max_z, "max");

                pcl[n] = new Vector3(x, y, z);
            }

            Debug.Log("pcl_Finished"+pcl[0]);
            //床生成
            floor_posi = new Vector3(max_x - min_x, min_y - 10.0f, max_z - min_z);
            Instantiate(floorObject, new Vector3(0.0f,min_y-10.0f,0.0f), Quaternion.identity);
            //obj.transform.scale = new Vector3(max_x-min_x, 10.0f, max_z-min_z);

            //0x0001*[0] 0x0010*

            //for (byte column = 0; column < width; column++)
            //{
            //    for(byte row = 0; row < height; row++)
            //    {
            //        int arrayPosition = column * point_step + row * row_step;

            //        //Debug.Log("x" + message.fields[0].offset);
            //        //Debug.Log("y" + message.fields[1].offset);
            //        //Debug.Log("z" + message.fields[2].offset);
            //        int x = arrayPosition + message.fields[0].offset;
            //        int y = arrayPosition + message.fields[1].offset;
            //        int z = arrayPosition + message.fields[2].offset;
            //        pcl[column * message.width + row] = new Vector3(message.data[column * message.point_step + row * message.row_step + message.fields[0].offset], message.data[y], message.data[z]);
            //        //Debug.Log(column * message.width + row);
            //        //Debug.Log("x" + pcl[column * width + row].x);
            //        //Debug.Log("y" + pcl[column * width + row].y);
            //        //Debug.Log("z" + pcl[column * width + row].z);
            //    }
            //}

            //int max = message.fields.GetLength(0);

            //for (int n = 0; n < max; n++)
            //{
            // Debug.Log("[" + n+"]"+ message.fields[n]);

            //}


        }

        void CreateMesh()
        {
            Debug.Log("CreateMesh" + pcl.GetLength(0));
            mesh = new Mesh();
            //Vector3[] points = pcl;
            int[] indecies = new int[size];
            Color[] colors = new Color[size];
            for (int i = 0; i < size; ++i)
            {
                //points[i] = new Vector3(byteArray[3*i], byteArray[3 * i + 1], byteArray[3 * i + 2]);
                indecies[i] = i;
                //colors[i] = new Color(i/size, i / size, i / size, 1.0f);
                colors[i] = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                Debug.Log("i" + i);
                Debug.Log("x" + pcl[i].x);
                Debug.Log("y" + pcl[i].y);
                Debug.Log("z" + pcl[i].z);
            }

            mesh.vertices = pcl;
            mesh.colors = colors;
            mesh.SetIndices(indecies, MeshTopology.Points, 0);

        }

        float ComFloat(float x, float last_x, string type)
        {
            if (type == "min")
            {
                if (x < last_x)
                {
                    return x;
                }
                else
                {
                    return last_x;
                }
            }else if(type == "max")
            {
                if (x > last_x)
                {
                    return x;
                }
                else
                {
                    return last_x;
                }
            }
            else
            {
                return 0.0f;
            }
        }
    }
}

