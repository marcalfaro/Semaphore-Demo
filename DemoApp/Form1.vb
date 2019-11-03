Option Explicit On
Imports System.Threading

Public Class Form1
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim ss As SemaphoreSlim = New SemaphoreSlim(0, 3)

        Console.WriteLine($"{ ss.CurrentCount } tasks can enter the semaphore.")

        Dim tasks As Task() = New Task(9) {}
        For i As Integer = 0 To 9
            tasks(i) = Task.Run(Sub()
                                    slowLongMethod(ss)
                                End Sub)
        Next

        'wait until all tasks are filled
        Console.WriteLine($"{ vbNewLine }Waiting 5secs to start...")
        Await Task.Delay(10000)

        'release # tasks so other tasks can continue
        Dim tasksToRelease As Integer = 3
        Console.Write($"{ vbNewLine }Main thread calls Release({ tasksToRelease }) --> ")
        ss.Release(tasksToRelease)
        Console.WriteLine($"{ ss.CurrentCount } tasks can enter the semaphore now.")

        'wait till all tasks are done
        Console.WriteLine($"{ vbNewLine }Waiting for all tasks to complete.")
        Task.WaitAll(tasks)

        Console.WriteLine("Main thread exits.")
    End Sub

    Private Sub slowLongMethod(ByVal ss As SemaphoreSlim)
        Console.WriteLine($"Task { Task.CurrentId } begins and waits for the semaphore.")
        ss.Wait()

        Console.WriteLine($"Task { Task.CurrentId } enters the semaphore.")
        Try

            'Really really slow and long procedure here
            'Thread.Sleep(2000)
            Task.Delay(2000)

        Catch ex As Exception

        Finally
            'When the task Is ready, release the semaphore. It Is vital to ALWAYS release the semaphore when we are ready, Or else we will end up with a Semaphore that Is forever locked.
            'This Is why it Is important to do the Release within a try...finally clause; program execution may crash Or take a different path, this way you are guaranteed execution

            Console.WriteLine($"Task { Task.CurrentId } releases the semaphore.")
            ss.Release()
        End Try
    End Sub

End Class
