import { DataTable } from "@/components/data-table";
import Layout from "@/components/layout";
import { ColumnDef } from "@tanstack/react-table";
import axios from "axios";
import { Metadata } from "next";
import { z } from "zod";
import { Employee, employeeSchema } from "../../data/schema";

export const metadata: Metadata = {
  title: "Tasks",
  description: "A task and issue tracker build using Tanstack Table.",
};

async function getEmployees() {
  try {
    const response = await axios.get("http://localhost:5000/odata/employees");
    const tasks = response.data.value;

    return z.array(employeeSchema).parse(tasks);
  } catch (error) {
    console.error("Error fetching tasks:", error);
    return [];
  }
}

const columns: ColumnDef<Employee>[] = [
  {
    accessorKey: "employeeName",
    header: "Employee Name",
  },
];

export default async function Page() {
  const employees = await getEmployees();
  return (
    <Layout>
      <div className="hidden h-full flex-1 flex-col space-y-8 p-8 md:flex">
        <div className="flex items-center justify-between space-y-2">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Welcome back!</h2>
            <p className="text-muted-foreground">
              Here&apos;s a list of your tasks for this month!
            </p>
          </div>
          <div className="flex items-center space-x-2">{/* <UserNav /> */}</div>
        </div>
        <DataTable data={employees} columns={columns} />
      </div>
    </Layout>
  );
}
