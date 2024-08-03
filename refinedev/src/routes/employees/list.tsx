import React from "react";

import { List, useTable } from "@refinedev/antd";
import { type HttpError, getDefaultFilter } from "@refinedev/core";

import { SearchOutlined } from "@ant-design/icons";
import { Form, Grid, Input, Radio, Space, Spin } from "antd";
import debounce from "lodash/debounce";

import { ListTitleButton } from "@/components";
import { TableView } from "./components/table-view";
import { t } from "i18next";
type Props = React.PropsWithChildren;

export enum EmployeePositionRole {
  Unknown = 0,
  AR = 1,
  UM = 2,
  SM = 3,
  DM = 4,
  Agency = 5,
}

type EmployeePositionRoleKeys = keyof typeof EmployeePositionRole;

export type DirectManager = {};

export type IEmployeePosition = {
  id: string;
  employeeId: string;
  beginDate: Date;
  positionRole: EmployeePositionRoleKeys;
};

export type IEmployee = {
  id: string;
  employeeId: string;
  employeeName: string;
  joiningDate: Date;
  position: string;
  positions: IEmployeePosition[];
  taxId: string;
  phoneNumber: string;
  email: string;
  identityNumber: string;
  directManager: string;
  directManagers: DirectManager[];
  introducer: string;
  createdAt: Date;
};

export const EmpployeesListPage: React.FC<Props> = ({ children }) => {
  const screens = Grid.useBreakpoint();

  const { tableProps, searchFormProps, filters, sorters, tableQueryResult } =
    useTable<IEmployee, HttpError, { name: string }>({
      dataProviderName: "odata",
      liveMode: "off",
      pagination: {
        pageSize: 10,
      },
      sorters: {
        initial: [
          {
            field: "createdAt",
            order: "desc",
          },
        ],
      },
      onSearch: (values) => {
        return [
          {
            field: "employeeName",
            operator: "contains",
            value: values.name,
          },
        ];
      },
      meta: {
        expand: "positions",
      },
    });

  const onSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    searchFormProps?.onFinish?.({
      name: e.target.value,
    });
  };
  const debouncedOnChange = debounce(onSearch, 500);

  return (
    <div className="page-container">
      <List
        breadcrumb={false}
        headerButtons={() => {
          return (
            <Space
              style={{
                marginTop: screens.xs ? "1.6rem" : undefined,
              }}
            >
              <Form
                {...searchFormProps}
                initialValues={{
                  name: getDefaultFilter("name", filters, "contains"),
                }}
                layout="inline"
              >
                <Form.Item name="name" noStyle>
                  <Input
                    size="large"
                    // @ts-expect-error Ant Design Icon's v5.0.1 has an issue with @types/react@^18.2.66
                    prefix={<SearchOutlined className="anticon tertiary" />}
                    suffix={
                      <Spin
                        size="small"
                        spinning={tableQueryResult.isFetching}
                      />
                    }
                    placeholder="Search by name"
                    onChange={debouncedOnChange}
                  />
                </Form.Item>
              </Form>
            </Space>
          );
        }}
        contentProps={{
          style: {
            marginTop: "28px",
          },
        }}
        title={
          <ListTitleButton toPath="employees" buttonText={t("employees.add")} />
        }
      >
        <TableView
          tableProps={tableProps}
          filters={filters}
          sorters={sorters}
        />
        {children}
      </List>
    </div>
  );
};
