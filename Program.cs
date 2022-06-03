using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MultiThreadedMergeSort
{
    internal class Program
    {

		static void merge(Int64[] arr, int l, int m, int r)
		{
			// Find sizes of two
			// subarrays to be merged
			int n1 = m - l + 1;
			int n2 = r - m;

			// Create temp arrays
			Int64[] L = new Int64[n1];
			Int64[] R = new Int64[n2];
			int i, j;

			// Copy data to temp arrays
			for (i = 0; i < n1; ++i)
				L[i] = arr[l + i];
			for (j = 0; j < n2; ++j)
				R[j] = arr[m + 1 + j];

			// Merge the temp arrays

			// Initial indexes of first
			// and second subarrays
			i = 0;
			j = 0;

			// Initial index of merged
			// subarray array
			int k = l;
			while (i < n1 && j < n2)
			{
				if (L[i] <= R[j])
				{
					arr[k] = L[i];
					i++;
				}
				else
				{
					arr[k] = R[j];
					j++;
				}
				k++;
			}
			// Copy remaining elements
			// of L[] if any
			while (i < n1)
			{
				arr[k] = L[i];
				i++;
				k++;
			}

			// Copy remaining elements
			// of R[] if any
			while (j < n2)
			{
				arr[k] = R[j];
				j++;
				k++;
			}
		}
		static void sort(Int64[] arr, int l, int r)
		{
			if (l < r)
			{
				// Find the middle
				// point
				int m = l + (r - l) / 2;

				// Sort first and
				// second halves
				sort(arr, l, m);
				sort(arr, m + 1, r);

				// Merge the sorted halves
				merge(arr, l, m, r);
			}
		}
		static void printArray(Int64[] arr)
		{
			int n = arr.Length;
			for (int i = 0; i < n; ++i)
				Console.Write(arr[i] + " ");
			Console.WriteLine();
		}

		public static Int64[] mergeSort(Int64[] array, int minElements)
        {
			if(minElements > array.Length)
            {
				return null;
            }

			//create threads with appropriate indexes
			int numOfThreads = (int)Math.Ceiling((double)array.Length/minElements);
			int last = 0;
			Thread [] threads = new Thread[numOfThreads];
			for (int i = 0; i < numOfThreads - 1; ++i)
            {
				int temp = i;
				threads[temp] = new Thread(() => sort(array, temp * minElements, (temp + 1) * minElements - 1));
				last = (temp + 1) * minElements;
			}
			threads[numOfThreads - 1] = new Thread(() => sort(array, last, array.Length - 1));

			//start and wait for all threads
			foreach (Thread thread in threads)
				thread.Start();

			foreach(Thread thread in threads)
				thread.Join();

			//now each thread has its own sorted subarrays, now we should merge it 
			printArray(array);

			//merge all subarrays
			int logOfThreads = (int)Math.Ceiling(Math.Log(numOfThreads, 2));
			int numOfElements = minElements;
			int oldL = 0;
			int newM = 0;
			for (int i = 0; i < logOfThreads; ++i)
			{
				//merge all subarrays on given log level
				int tempI = i;
				numOfThreads = (int)Math.Ceiling((double)numOfThreads / 2);
				Thread[] subThreads = new Thread[numOfThreads];
				int oldr = 0;
				numOfElements *= 2;
				for (int j = 0; j < numOfThreads - 1; ++j)
				{

					int tempJ = j;

					int l = oldr;
					int r = l + numOfElements - 1;
					int m = l + (r - l) / 2;

					oldL = l;
					oldr = r + 1;

					subThreads[tempJ] = new Thread(() => merge(array, l, m, r));
				}
				//merge last thread 
				int ll = oldr;
				int mm = 0;
				int rr = array.Length - 1;
				if (newM != 0)
				{
					mm = newM;
				}
				else
				{
					mm = ll + (rr - ll) / 2;
				}
				subThreads[numOfThreads - 1] = new Thread(() => merge(array, ll, mm, rr));
				newM = ll-1;

				//run and wait for all threads with subarrays
				for (int j = 0; j < numOfThreads; ++j)
                {
					int tempJ = j;
					subThreads[tempJ].Start();
				}
				for (int j = 0; j < numOfThreads; ++j)
				{
					int tempJ = j;
					subThreads[tempJ].Join();
				}
			}
			
			printArray(array);
			return array;
        }

		static int arraySortedOrNot(Int64[] arr, int n)
		{
			// Array has one or no element or the
			// rest are already checked and approved.
			if (n == 1 || n == 0)
				return 1;

			// Unsorted pair found (Equal values allowed)
			if (arr[n - 1] < arr[n - 2])
			{
				return 0;
			}

			// Last pair was sorted
			// Keep on checking
			return arraySortedOrNot(arr, n - 1);
		}

		static void Main(string[] args)
		{
			int Min = 0;
			int Max = 10000;

			// this declares an integer array with 5 elements
			// and initializes all of them to their default value
			// which is zero
			for (int k = 0; k < 100; k++)
			{
				Int64[] arr = new Int64[k];

				Random randNum = new Random();
				for (int i = 0; i < arr.Length; i++)
				{
					arr[i] = randNum.Next(Min, Max);
				}


				for (int i = 1; i < arr.Length + 1; i++)
				{
					mergeSort(arr, i);
					if (arraySortedOrNot(arr, arr.Length) == 1)
					{
						Console.WriteLine(i);
						Console.WriteLine("Nice");
					}
					else
					{
						Console.WriteLine(i);
						Console.WriteLine("NO!");
						Console.ReadLine();
					}
				}
			}
		}
    }
}

