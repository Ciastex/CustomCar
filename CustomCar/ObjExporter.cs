﻿using System.IO;
using System.Text;
using UnityEngine;

public class ObjExporter
{
    public static string MeshToString(Mesh m)
    {
        var sb = new StringBuilder();

        sb.Append("g ").Append(m.name).Append("\n");
        foreach (var v in m.vertices) sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        sb.Append("\n");
        foreach (var v in m.normals) sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        sb.Append("\n");
        foreach (Vector3 v in m.uv) sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        for (var material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            sb.Append("usemtl ").Append("mat" + material).Append("\n");
            sb.Append("usemap ").Append("mat" + material).Append("\n");

            sb.Append("\ng " + m.name + (material + 1) + "\n\n");

            var triangles = m.GetTriangles(material);
            for (var i = 0; i < triangles.Length; i += 3)
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
        }

        return sb.ToString();
    }

    public static void MeshToFile(Mesh m, string filename)
    {
        using (var sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(m));
        }
    }

    public static void saveTextureToFile(Texture texture, string filename)
    {
        var texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        var currentRT = RenderTexture.active;

        var renderTexture = new RenderTexture(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        var pixels = texture2D.GetPixels();

        RenderTexture.active = currentRT;

        var bytes = texture2D.EncodeToPNG();
        var file = File.Open(Application.dataPath + "/exportedTextures/" + filename, FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }
}