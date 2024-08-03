import { DeleteButton, ShowButton } from "@refinedev/antd";
import { type CrudFilters, type CrudSorting } from "@refinedev/core";

import { Button, Space, Table, type TableProps } from "antd";

import { PaginationTotal, Text } from "@/components";
import { ContactStatusEnum } from "@/enums";
import { useCompaniesSelect } from "@/hooks/useCompaniesSelect";
import { IEmployee } from "../list";
import { useTranslation } from "react-i18next";
import dayjs from "dayjs";
import { EmployeePositionCell } from "./position-cell";

type Props = {
  tableProps: TableProps<IEmployee>;
  filters: CrudFilters;
  sorters: CrudSorting;
};

export const TableView: React.FC<Props> = ({ tableProps }) => {
  const {
    i18n: { t },
  } = useTranslation();

  const handlePositionClick = (record: IEmployee) => {
    console.log("Button clicked", record);
    // Add your custom logic here, e.g., navigate to a different page or open a modal
  };
  return (
    <Table
      {...tableProps}
      pagination={{
        ...tableProps.pagination,
        pageSizeOptions: ["5", "10", "15", "20"],
        showSizeChanger: true,
        showTotal: (total) => (
          <PaginationTotal total={total} entityName="employees" />
        ),
      }}
      rowKey="id"
    >
      <Table.Column
        dataIndex="employeeName"
        title={t("employees.employeeName")}
        width={200}
      />
      <Table.Column
        dataIndex={"employeeId"}
        title={t("employees.employeeId")}
      />
      <Table.Column dataIndex="email" title={t("employees.email")} />
      <Table.Column
        dataIndex={"joiningDate"}
        title={t("employees.joiningDate")}
        render={(value) => {
          return <Text>{dayjs(value).fromNow()}</Text>;
        }}
      />
      <Table.Column
        dataIndex={"phoneNumber"}
        title={t("employees.phoneNumber")}
      />
      <Table.Column
        dataIndex={"identityNumber"}
        title={t("employees.identityNumber")}
      />
      <Table.Column<IEmployee>
        dataIndex=""
        title={t("employees.position")}
        render={(_, record) => <EmployeePositionCell record={record} />}
      />

      <Table.Column
        dataIndex={"directManager"}
        title={t("employees.directManager")}
      />
      <Table.Column
        dataIndex={"introducer"}
        title={t("employees.introducer")}
      />
      <Table.Column<IEmployee>
        fixed="right"
        title={t("table.actions")}
        dataIndex="actions"
        render={(_, record) => (
          <Space>
            <ShowButton hideText size="small" recordItemId={record.id} />
            <DeleteButton
              dataProviderName="odata"
              hideText
              size="small"
              recordItemId={record.id}
            />
          </Space>
        )}
      />
    </Table>
  );
};
