import { useState } from "react";

import {
  Button,
  Form,
  Input,
  Modal,
  Popconfirm,
  Select,
  Table,
  Typography,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { Text } from "@/components";
import { EmployeePositionRole, IEmployee, IEmployeePosition } from "../list";
import dayjs from "dayjs";
import { useTranslation } from "react-i18next";
import { join, orderBy } from "lodash";
import { LocaleDatePicker } from "@/components/locale-date-picker";
import { getEnumOptions, uuidEmpty } from "@/utilities/enum-utils";
import { useCreate } from "@refinedev/core";

export const EmployeePositionCell = ({ record }: { record: IEmployee }) => {
  const [opened, setOpened] = useState(false);
  const [editingKey, setEditingKey] = useState("");
  const [form] = Form.useForm();

  const [dataSource, setDataSource] = useState<IEmployeePosition[]>([
    ...orderBy(record.positions, ["beginDate"], ["desc"]),
  ]);

  const { t } = useTranslation();

  const handleSave = (abc: any) => {
    console.log("handleSave", abc);
  };

  const edit = (record: any) => {
    console.log("edit", record);

    form.setFieldsValue({ ...record });
    setEditingKey(record.id);
  };

  const setDefaultDataSource = () =>
    setDataSource([...orderBy(record.positions, ["beginDate"], ["desc"])]);

  const cancel = () => {
    setEditingKey("");
  };

  const isEditing = (record: IEmployeePosition) => record.id === editingKey;

  const handleDelete = (record: IEmployeePosition) => {};

  const roleOptions = getEnumOptions(EmployeePositionRole);

  const { mutate } = useCreate();

  const save = async () => {
    const row = form.getFieldsValue();
    mutate({
      values: row,
      resource: "employees",
      meta: {
        childResource: `${record.id}/positions`,
      },
      dataProviderName: "odata",
      successNotification: {
        message: "",
        description: "Position updated successfully",
        type: "success",
      },
    });
  };

  const columns: ColumnsType<IEmployee["positions"][0]> = [
    {
      title: t("commonDates.beginDate"),
      dataIndex: "beginDate",
      key: "beginDate",
      render: (value) => <Text>{dayjs(value).fromNow()}</Text>,
      width: "20%",
      onCell: (record: IEmployeePosition) => ({
        record,
        editable: true,
        inputType: "date",
        dataIndex: "beginDate",
        title: "abc",
        handleSave,
        editing: isEditing(record),
        control: <LocaleDatePicker />,
      }),
    },
    {
      title: t("employees.position"),
      dataIndex: "positionRole",
      key: "positionRole",
      onCell: (record: IEmployeePosition) => ({
        record,
        editable: true,
        inputType: "text",
        dataIndex: "positionRole",
        title: "abc",
        handleSave,
        editing: isEditing(record),
        control: <Select options={roleOptions} />,
      }),
    },
    {
      title: "operation",
      dataIndex: "operation",
      render: (_: any, record: IEmployeePosition) => {
        const editable = isEditing(record);
        return editable ? (
          <span>
            <Typography.Link onClick={() => save()} style={{ marginRight: 8 }}>
              Save
            </Typography.Link>
            <Popconfirm title="Sure to cancel?" onConfirm={cancel}>
              <a>Cancel</a>
            </Popconfirm>
          </span>
        ) : (
          <Typography.Link
            disabled={editingKey !== ""}
            onClick={() => edit(record)}
          >
            Edit
          </Typography.Link>
        );
      },
    },
  ];

  return (
    <div>
      <div>
        <div style={{ display: "flex", justifyContent: "flex-start" }}>
          <Button
            type="link"
            style={{ padding: 0, textAlign: "left" }}
            onClick={() => setOpened((prev) => !prev)}
          >
            {join(
              record.positions.map((x) => x.positionRole),
              ", "
            )}
          </Button>
        </div>
      </div>
      {opened && (
        <Modal
          cancelButtonProps={{
            onClick: () => {
              setOpened(false);
              setDefaultDataSource();
            },
          }}
          footer={(or1, e) => {
            return (
              <>
                <Button
                  onClick={() => {
                    setDataSource([
                      ...dataSource,
                      {
                        id: uuidEmpty,
                        beginDate: new Date(),
                        positionRole: "AR",
                        employeeId: record.employeeId,
                      },
                    ]);
                  }}
                >
                  {t("employees.addPosition")}
                </Button>
                {or1}
              </>
            );
          }}
          open={opened}
          onOk={() => setOpened(false)}
          onCancel={() => setOpened(false)}
          style={{ minWidth: "40vw" }}
          bodyStyle={{
            maxHeight: "300px",
            overflow: "auto",
          }}
        >
          <Form form={form} component={false}>
            <Table
              components={{
                body: {
                  cell: EditableCell,
                },
              }}
              dataSource={dataSource}
              pagination={false}
              rowKey="field"
              bordered
              size="small"
              scroll={{ x: true }}
              columns={columns}
            />
          </Form>
        </Modal>
      )}
    </div>
  );
};

const EditableCell: React.FC<React.PropsWithChildren<EditableCellProps>> = ({
  editing,
  dataIndex,
  title,
  record,
  index,
  children,
  control,
  ...restProps
}) => {
  return (
    <td {...restProps}>
      {editing ? (
        <Form.Item
          name={dataIndex}
          style={{ margin: 0 }}
          rules={[
            {
              required: true,
              message: `Please Input ${title}!`,
            },
          ]}
        >
          {control ? control : <Input />}
        </Form.Item>
      ) : (
        children
      )}
    </td>
  );
};

interface EditableCellProps extends React.HTMLAttributes<HTMLElement> {
  editing: boolean;
  dataIndex: string;
  title: any;
  record: IEmployeePosition;
  index: number;
  control?: JSX.Element;
}
