using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Color[] PointColors = {Color.red, Color.green, Color.blue, Color.yellow};

    [SerializeField, Range(0,16)]
    int dimensions;

    float[,] vertices;
    bool active = false;

    private void Start()
    {
        float[,] input_vectors = new float[1, 2];
        input_vectors[0, 0] = 1;
        input_vectors[0, 1] = 0;
        GetNormalVector(input_vectors);
        CreateSimplex();
    }

    void CreateSimplex()
    {
        // 5 vertices for 4D
        vertices = new float[dimensions+1, dimensions];
        if (dimensions == 0) {
            active = true;
            return;
        }

        // First must be a unit vector
        vertices[0, 0] = 1f;

        // Dot-product must be -1/n. First property of simplices. Because of the unit vector x = -1/n
        for (int i = 1; i < dimensions + 1; i++)
        {
            // The starting values stay the same once the dot product is taken. 
            // This means the variables can be copied from the prvious vector
            for (int j = 0; j < i - 1; j++)
            {
                vertices[i, j] = vertices[i - 1, j];
            }

            if (i == dimensions)
            {
                vertices[i, i - 1] = -vertices[i - 1, i - 1];
                return;
            }

            // Calculate the dot product, this gives the value for i-1
            float total_dot = 0;
            for (int j = 0; j < i - 1; j++)
            {
                total_dot += Mathf.Pow(vertices[i - 1, j], 2);
            }

            // Simplified dot product where I calculate the missing element
            total_dot = (-1f / dimensions - total_dot) / vertices[i - 1, i - 1];
            vertices[i, i - 1] = total_dot;

            // Second must be unit vector. We leave the last one at 0. So a²+b²=c²
            // length(c) is 1, a = -1/3. b = sqrt(1^2 - (-1/3)^2)
            // Simplify: b = sqrt(9/9 - 1/9)
            // Simplify: b = sqrt(8) / sqrt(9)
            // Simplify: b = sqrt(8) / 3
            float y_squared = 0;
            for (int j = 0; j < i; j++)
            {
                y_squared += Mathf.Pow(vertices[i, j], 2);
            }

            vertices[i, i] = Mathf.Sqrt(1f - y_squared);
        }

        active = true;
    }

    void VectorProject()
    {
        float[,] Mtransform = new float[dimensions + 1, dimensions + 1];
        for (int i = 0; i < dimensions + 1; i++)
        {
            for (int j = 0; j < dimensions + 1; j++)
            {
                Mtransform[i, j] = (i == j) ? 1 : 0;

                if (i == dimensions && j != dimensions)
                {
                    // Fill last column with position vector
                }
            }
        }

        float[,] Mlookat = new float[dimensions + 1, dimensions + 1];

        // Precalculate the view angle
        float fov_angle = (1f / Mathf.Tan(Camera.main.fieldOfView));

        float[,] Mproject = new float[dimensions + 1, dimensions + 1];
        for (int i = 0; i < dimensions + 1; i++)
        {
            for (int j = 0; j < dimensions + 1; j++)
            {
                if (i == j && i < dimensions - 1)
                {
                    Mtransform[i, j] = (i == j) ? fov_angle : 0;
                } else
                {
                    Mtransform[i, j] = (i == j) ? 1 : 0;
                }
            }
        }

        
    }

    void OnDrawGizmosSelected()
    {
        if (active) {
            Vector3[] vectors = new Vector3[dimensions+1];

            for (int i = 0; i < dimensions + 1; i++)
            {
                vectors[i] = new Vector3(dimensions > 0 ? vertices[i, 0] : 0, 
                                         dimensions > 1 ? vertices[i, 1] : 0, 
                                         dimensions > 2 ? vertices[i, 2] : 0);
            }

            // Draw a yellow sphere at the transform's position
            for (int i = 0; i < vectors.Length; i++)
            {
                Gizmos.color = PointColors[Mathf.Min(i, PointColors.Length-1)];
                Gizmos.DrawSphere(vectors[i], 0.1f);
            }
        }
    }

    float[] GetNormalVector(float[,] input_vectors)
    {
        // The matrix we apply the vectors to
        float[,] matrix = new float[dimensions, dimensions];
        // Base vectors are all unit vectors for the dimension over all it's axis
        // Example 3D: 1 0 0, 0 1 0, 0 0 1 
        float[][] base_vectors = new float[dimensions][];

        for (uint i = 0; i < dimensions - 1; i++)
        {
            base_vectors[i] = new float[dimensions];
        }

        for (uint i = 0; i < dimensions - 1; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                // Fill the matrix with the iput vectors
                // in a column major style
                matrix[i, j] = input_vectors[i, j];
                // Fill the base vector 
                base_vectors[i][j] = (i == j) ? 1 : 0;
            }
        }

        for (uint j = 0; j < dimensions; j++)
        {
            int i = dimensions - 1;
            base_vectors[i][j] = i == j ? 1 : 0;
        }

        float[] normal_vector = new float[dimensions];

        for (uint i = 0; i < dimensions; i++)
        {
            float[,] s = new float[dimensions - 1, dimensions - 1];
            for (int j = 0, r = 0; j < dimensions-1; j++, r++)
            {
                for (int k = 0, c = 0; k < dimensions; k++)
                {
                    if (k == i)
                    {
                        continue;
                    }

                    s[r, c] = matrix[j, k];

                    c++;
                }
            }

            if ((i % 2) == 0)
            {
                normal_vector = VecMatHelper.VecAdd(normal_vector, 
                                                    VecMatHelper.VecScale(base_vectors[i], 
                                                                          VecMatHelper.MatDetermant(s)));
            }
            else
            {
                normal_vector = VecMatHelper.VecSub(normal_vector,
                                                    VecMatHelper.VecScale(base_vectors[i],
                                                                          VecMatHelper.MatDetermant(s)));
            }
        }

        return normal_vector;
    }

    // Update is called once per frame
    void OnValidate () {
        CreateSimplex();
	}


    float[,] GetLookatMatrix(float[] from, float[] to)
    {
        float[,] matrix = new float[dimensions, dimensions];
        float[,] orthogonal_vectors = new float[dimensions - 2, dimensions];
        for (int i = 0; i < (dimensions - 2); i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                orthogonal_vectors[i, j] = (((i + 1) == j) ? 1 : 0);
            }
        }

        float[,] columns = new float[dimensions, dimensions];
        columns = VecMatHelper.MatSetVecAtCol(columns, VecMatHelper.VecNorm(VecMatHelper.VecSub(to, from)), dimensions - 1);

        for (int i = 0; i < (dimensions - 1); i++)
        {
            float[,] cross_vectors = new float[dimensions - 1, dimensions];

            for (int j = i - (dimensions - 2), c = 0; c < (dimensions - 1); j++, c++)
            {
                if (j < 0)
                {
                    cross_vectors = VecMatHelper.MatSetVecAtCol(cross_vectors, orthogonal_vectors[(j + (dimensions - 2))], c);
                }
                else if (j == 0)
                {
                    VecMatHelper.MatSetVecAtCol(cross_vectors = columns[(dimensions - 1)], c);
                }
                else
                {
                    VecMatHelper.MatSetVecAtCol(cross_vectors, columns[(j - 1)], c);
                }
            }

            VecMatHelper.MatSetVecAtCol(columns, GetNormalVector(cross_vectors), i);

            if (i != (dimensions - 2))
            {
                VecMatHelper.MatSetVecAtCol(columns, GetNormalVector(columns[i]), i);
            }
        }

        for (int i = 0; i <= dimensions; i++)
        {
            for (int j = 0; j <= dimensions; j++)
            {
                if ((i < dimensions) && (j < dimensions))
                {
                    matrix[i, j] = columns[j, i];
                }
                else if (i == j)
                {
                    matrix[i, j] = 1;
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }

        return matrix;
    }

    float[,] GetPerspectiveMatrix()
    {
        float[,] matrix = new float[dimensions, dimensions];
        float fov = 1f / Mathf.Tan(Camera.main.fieldOfView / 2f);

        for (int i = 0; i <= dimensions; i++)
        {
            for (int j = 0; j <= dimensions; j++)
            {
                if (i == j)
                {
                    matrix[i, j] = ((i >= (dimensions - 1)) ? 1f : fov);
                }
                else
                {
                    matrix[i, j] = 0;
                }
            }
        }

        return matrix;
    }

    void GetProjectionMatrix()
    {
        float[] from = { Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z, 1f };
        float[] to = { Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z, 0 };

        float[,] Mview = new float[dimensions, dimensions];
        float[,] MlookAt = GetLookatMatrix(from, to);
        float[,] Mperspective = GetPerspectiveMatrix();

        Mview = VecMatHelper.MatMultiply(MlookAt, Mperspective);
    }
}

public static class VecMatHelper
{
    public static float[] VecAdd(float[] v1, float[] v2)
    {
        float[] ret_vec = new float[v1.Length];

        for (int i = 0; i < v1.Length; i++)
        {
            ret_vec[i] = v1[i] + v2[i];
        }

        return ret_vec;
    }

    public static float[] VecSub(float[] v1, float[] v2)
    {
        float[] ret_vec = new float[v1.Length];

        for (int i = 0; i < v1.Length; i++)
        {
            ret_vec[i] = v1[i] - v2[i];
        }

        return ret_vec;
    }

    public static float[] VecNorm(float[] v1)
    {
        float[] ret_vec = new float[v1.Length];

        float total_length = 0;
        for (int i = 0; i < v1.Length; i++) {
            total_length += Mathf.Pow(v1[i], 2);
        }

        float length = Mathf.Sqrt(total_length);

        for (int i = 0; i < v1.Length; i++)
        {
            ret_vec[i] = v1[i] / length;
        }

        return ret_vec;
    }

    public static float[] VecScale(float[] v1, float scale)
    {
        float[] ret_vec = new float[v1.Length];
        for (int i = 0; i < v1.Length; i++)
        {
            ret_vec[i] = v1[i] * scale;
        }

        return ret_vec;
    }

    public static float MatDetermant(float[,] input_matrix)
    {
        float rv = 0;
        int dimensions = input_matrix.GetLength(0);

        for (int i = 0; i < dimensions; i++)
        {
            float[,] s = new float[dimensions - 1, dimensions - 1];

            for (int j = 1, r = 0; j < dimensions; j++, r++)
            {
                for (int k = 0, c = 0; k < dimensions; k++)
                {
                    if (k == i)
                    {
                        continue;
                    }

                    s[r, c] = input_matrix[j, k];

                    c++;
                }
            }

            if (i == 0)
            {
                rv = input_matrix[0, i] * MatDetermant(s);
            }
            else if ((i % 2) == 0)
            {
                rv += input_matrix[0, i] * MatDetermant(s);
            }
            else
            {
                rv -= input_matrix[0, i] * MatDetermant(s);
            }
        }

        return rv;
    }

    public static float[,] MatSetVecAtCol(float[,] m1, float[] v1, int col)
    {
        float[,] matrix = m1;

        for (int i = 0; i < m1.GetLength(1); i++)
        {
            matrix[col, i] = v1[i];
        }

        return matrix;
    }

    public static float[,] MatMultiply(float[,] m1, float[,] m2)
    {
        float[,] matrix = new float[m1.GetLength(0), m1.GetLength(1)];

        for (int i = 0; i < m1.GetLength(0); i++)
        {
            for (int j = 0; j < m1.GetLength(1); j++)
            {
                matrix[i, j] = m1[i, 0] * m2[0, j];

                for (int k = 0; k < m1.GetLength(1); k++)
                {
                    matrix[i, j] += m1[i, j] * m2[i, j];
                }
            }
        }

        return matrix;
    }
}
