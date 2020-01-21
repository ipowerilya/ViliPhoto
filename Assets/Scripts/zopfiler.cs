using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;

public class zopfiler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WebClient client = new WebClient();

        Stream data = client.OpenRead(@"https://yadi.sk/d/YndCWquIguNqaQ");
        StreamReader reader = new StreamReader(data);
        string s = reader.ReadToEnd();
        Console.WriteLine(s);
        data.Close();
        reader.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
