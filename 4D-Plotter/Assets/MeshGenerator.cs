using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Color[] PointColors = {Color.red, Color.green, Color.blue, Color.yellow};

    [SerializeField, Range(0,16)]
    int dimensions;

    VectorN[] vertices;
    bool active = false;

    private void Start()
    {
        CreateSimplex();
    }

    void CreateSimplex()
    {
        // 5 vertices for 4D
        vertices = new VectorN[dimensions+1];
        if (dimensions == 0) {
            active = true;
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new VectorN(dimensions);
        }

        // First must be a unit vector
        vertices[0][0] = 1f;

        // Dot-product must be -1/n. First property of simplices. Because of the unit vector x = -1/n
        for (int i = 1; i < dimensions + 1; i++)
        {
            // The starting values stay the same once the dot product is taken. 
            // This means the variables can be copied from the prvious vector
            for (int j = 0; j < i - 1; j++)
            {
                vertices[i][j] = vertices[i - 1][j];
            }

            if (i == dimensions)
            {
                vertices[i][i - 1] = -vertices[i - 1][i - 1];
                return;
            }

            // Calculate the dot product, this gives the value for i-1
            float total_dot = 0;
            for (int j = 0; j < i - 1; j++)
            {
                total_dot += Mathf.Pow(vertices[i - 1][j], 2);
            }

            // Simplified dot product where I calculate the missing element
            total_dot = (-1f / dimensions - total_dot) / vertices[i - 1][i - 1];
            vertices[i][i - 1] = total_dot;

            // Second must be unit vector. We leave the last one at 0. So a²+b²=c²
            // length(c) is 1, a = -1/3. b = sqrt(1^2 - (-1/3)^2)
            // Simplify: b = sqrt(9/9 - 1/9)
            // Simplify: b = sqrt(8) / sqrt(9)
            // Simplify: b = sqrt(8) / 3
            float y_squared = 0;
            for (int j = 0; j < i; j++)
            {
                y_squared += Mathf.Pow(vertices[i][j], 2);
            }

            vertices[i][i] = Mathf.Sqrt(1f - y_squared);
        }

        active = true;
    }

    void OnDrawGizmosSelected()
    {
        if (active) {
            Vector3[] vectors = new Vector3[dimensions+1];

            for (int i = 0; i < dimensions + 1; i++)
            {
                vectors[i] = new Vector3(dimensions > 0 ? vertices[i][0] : 0, 
                                         dimensions > 1 ? vertices[i][1] : 0, 
                                         dimensions > 2 ? vertices[i][2] : 0);
            }

            // Draw a yellow sphere at the transform's position
            for (int i = 0; i < vectors.Length; i++)
            {
                Gizmos.color = PointColors[Mathf.Min(i, PointColors.Length-1)];
                Gizmos.DrawSphere(vectors[i], 0.1f);
            }
        }
    }

    // Update is called once per frame
    void OnValidate () {
        CreateSimplex();
	}


    MatrixNxN GetLookatMatrix(VectorN from, VectorN to)
    {
        MatrixNxN matrix = new MatrixNxN(dimensions);
        VectorN[] orthogonal_vectors = new VectorN[dimensions - 2];

        for (int i = 0; i < (dimensions - 2); i++)
        {
            orthogonal_vectors[i] = new VectorN(dimensions);
            for (int j = 0; j < dimensions; j++)
            {
                orthogonal_vectors[i][j] = (((i + 1) == j) ? 1 : 0);
            }
        }
        
        MatrixNxN columns = new MatrixNxN(dimensions);
        columns[dimensions-1] = VectorN.Normalize(VectorN.Subtract(to, from));

        for (int i = 0; i < (dimensions - 1); i++)
        {
            VectorN[] cross_vectors = new VectorN[dimensions - 1];
            for (int j = 0; j < cross_vectors.Length; j++)
            {
                cross_vectors[i] = new VectorN(dimensions);
            }

            for (int j = i - (dimensions - 2), c = 0; c < (dimensions - 1); j++, c++)
            {
                if (j < 0)
                {
                    cross_vectors[c] = orthogonal_vectors[(j + (dimensions - 2))];
                }
                else if (j == 0)
                {
                    cross_vectors[c] = columns[(dimensions - 1)];
                }
                else
                {
                    cross_vectors[c] = columns[(j - 1)];
                }
            }

            columns[i] = VectorN.GetNormal(cross_vectors);

            if (i != (dimensions - 2))
            {
                columns[i] = VectorN.Normalize(columns[i]);
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

    MatrixNxN GetPerspectiveMatrix()
    {
        MatrixNxN matrix = new MatrixNxN(dimensions);
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
        VectorN from = new VectorN(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z, 1f);
        VectorN to = new VectorN(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z, 0);

        MatrixNxN Mview = new MatrixNxN(dimensions);
        MatrixNxN MlookAt = GetLookatMatrix(from, to);
        MatrixNxN Mperspective = GetPerspectiveMatrix();

        Mview = MatrixNxN.Multiply(MlookAt, Mperspective);
    }
}

struct VectorN
{
    private float[] data;

    public VectorN(params float[] data)
    {
        this.data = data;
    }

    public VectorN(int size)
    {
        this.data = new float[size];
    }

    public int GetLength()
    {
        return data.Length;
    }
    
    // Indexer declaration.
    // If index is out of range, the temps array will throw the exception.
    public float this[int index]
    {
        get
        {
            return data[index];
        }

        set
        {
            data[index] = value;
        }
    }

    public void Add(VectorN v2)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] += v2[i];
        }
    }

    public void Subtract(VectorN v2)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] -= v2[i];
        }
    }

    public void Normalize()
    {
        float total_length = 0;
        for (int i = 0; i < data.Length; i++)
        {
            total_length += Mathf.Pow(data[i], 2);
        }

        float length = Mathf.Sqrt(total_length);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] /= length;
        }
    }

    public void Scale(float scale)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] *= scale;
        }
    }

    // Static

    public static VectorN Add(VectorN v1, VectorN v2)
    {
        VectorN ret_vec = new VectorN(v1.GetLength());

        for (int i = 0; i < v1.GetLength(); i++)
        {
            ret_vec[i] = v1[i] + v2[i];
        }

        return ret_vec;
    }

    public static VectorN Subtract(VectorN v1, VectorN v2)
    {
        VectorN ret_vec = new VectorN(v1.GetLength());

        for (int i = 0; i < v1.GetLength(); i++)
        {
            ret_vec[i] = v1[i] - v2[i];
        }

        return ret_vec;
    }

    public static VectorN Normalize(VectorN v1)
    {
        VectorN ret_vec = new VectorN(v1.GetLength());

        float total_length = 0;
        for (int i = 0; i < v1.GetLength(); i++)
        {
            total_length += Mathf.Pow(v1[i], 2);
        }

        float length = Mathf.Sqrt(total_length);

        for (int i = 0; i < v1.GetLength(); i++)
        {
            ret_vec[i] = v1[i] / length;
        }

        return ret_vec;
    }

    public static VectorN Scale(VectorN v1, float scale)
    {
        VectorN ret_vec = new VectorN(v1.GetLength());
        for (int i = 0; i < v1.GetLength(); i++)
        {
            ret_vec[i] = v1[i] * scale;
        }

        return ret_vec;
    }

    public static VectorN GetNormal(VectorN[] input_vectors)
    {
        int dimensions = input_vectors[0].GetLength();

        // The matrix we apply the vectors to
        MatrixNxN matrix = new MatrixNxN(dimensions);
        // Base vectors are all unit vectors for the dimension over all it's axis
        // Example 3D: 1 0 0, 0 1 0, 0 0 1 
        VectorN[] base_vectors = new VectorN[dimensions];

        for (uint i = 0; i < dimensions - 1; i++)
        {
            base_vectors[i] = new VectorN(dimensions);
        }

        for (int i = 0; i < dimensions - 1; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                // Fill the matrix with the iput vectors
                // in a column major style
                matrix[i, j] = input_vectors[i][j];
                // Fill the base vector 
                base_vectors[i][j] = (i == j) ? 1 : 0;
            }
        }

        for (int j = 0; j < dimensions; j++)
        {
            int i = dimensions - 1;
            base_vectors[i][j] = i == j ? 1 : 0;
        }

        VectorN normal_vector = new VectorN(dimensions);

        for (int i = 0; i < dimensions; i++)
        {
            MatrixNxN s = new MatrixNxN(dimensions - 1);
            for (int j = 0, r = 0; j < dimensions - 1; j++, r++)
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
                normal_vector.Add(Scale(base_vectors[i], MatrixNxN.Determant(s)));
            }
            else
            {
                normal_vector.Subtract(Scale(base_vectors[i], MatrixNxN.Determant(s)));
            }
        }

        return normal_vector;
    }
}
 

struct MatrixNxN
{
    VectorN[] data;

    public MatrixNxN(params float[] data)
    {
        int size = (int)Mathf.Sqrt(data.Length);
        this.data = new VectorN[size];

        for (int i = 0; i < size; i++)
        {
            this.data[i] = new VectorN(size);
            this.data[i][i%size] = data[i];
        }
    }

    public MatrixNxN(int size)
    {
        this.data = new VectorN[size];

        for (int i = 0; i < size; i++)
        {
            this.data[i] = new VectorN(size);
        }
    }

    // Indexer declaration.
    // If index is out of range, the temps array will throw the exception.
    public float this[int column, int row]
    {
        get
        {
            return data[column][row];
        }

        set
        {
            data[column][row] = value;
        }
    }

    public VectorN this[int column]
    {
        get
        {
            return data[column];
        }

        set
        {
            data[column] = value;
        }
    }

    public float Determant()
    {
        float determant = 0;
        int dimensions = data.Length;

        for (int i = 0; i < dimensions; i++)
        {
            MatrixNxN s = new MatrixNxN(dimensions - 1);

            for (int j = 1, r = 0; j < dimensions; j++, r++)
            {
                for (int k = 0, c = 0; k < dimensions; k++)
                {
                    if (k == i)
                    {
                        continue;
                    }

                    s[r, c] = this[j, k];

                    c++;
                }
            }

            if (i == 0)
            {
                determant = this[0, i] * Determant(s);
            }
            else if ((i % 2) == 0)
            {
                determant += this[0, i] * Determant(s);
            }
            else
            {
                determant -= this[0, i] * Determant(s);
            }
        }

        return determant;
    }

    public void SetVecAtCol(VectorN v1, int col)
    {
        this[col] = v1;
    }

    public void Multiply(MatrixNxN m2)
    {
        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data.Length; j++)
            {
                this[i, j] = this[i, 0] * m2[0, j];

                for (int k = 0; k < data.Length; k++)
                {
                    this[i, j] += this[i, j] * m2[i, j];
                }
            }
        }
    }

    public int GetDimension()
    {
        return data.Length;
    }

    // Static
    public static float Determant(MatrixNxN input_matrix)
    {
        float rv = 0;
        int dimensions = input_matrix.GetDimension();

        for (int i = 0; i < dimensions; i++)
        {
            MatrixNxN s = new MatrixNxN(dimensions - 1);

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
                rv = input_matrix[0, i] * MatrixNxN.Determant(s);
            }
            else if ((i % 2) == 0)
            {
                rv += input_matrix[0, i] * MatrixNxN.Determant(s);
            }
            else
            {
                rv -= input_matrix[0, i] * MatrixNxN.Determant(s);
            }
        }

        return rv;
    }

    public static MatrixNxN SetVecAtCol(MatrixNxN m1, VectorN v1, int col)
    {
        MatrixNxN matrix = new MatrixNxN(m1.GetDimension());

        matrix[col] = v1;

        return matrix;
    }

    public static MatrixNxN Multiply(MatrixNxN m1, MatrixNxN m2)
    {
        MatrixNxN matrix = new MatrixNxN(m1.GetDimension());

        for (int i = 0; i < m1.GetDimension(); i++)
        {
            for (int j = 0; j < m1.GetDimension(); j++)
            {
                matrix[i, j] = m1[i, 0] * m2[0, j];

                for (int k = 0; k < m1.GetDimension(); k++)
                {
                    matrix[i, j] += m1[i, j] * m2[i, j];
                }
            }
        }

        return matrix;
    }
}