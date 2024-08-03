import { useState } from "react";

import { Button, Modal, Table } from "antd";
import type { ColumnsType } from "antd/es/table";

import { Text } from "@/components";
import { IEmployee } from "../list";
import dayjs from "dayjs";
import { useTranslation } from "react-i18next";
import { orderBy } from "lodash";

export const EmployeeDirectManagerCell = ({
  record,
}: {
  record: IEmployee;
}) => {
  const [opened, setOpened] = useState(false);

  const { t } = useTranslation();

  const columns: ColumnsType<IEmployee["positions"][0]> = [
    {
      title: t("commonDates.beginDate"),
      dataIndex: "beginDate",
      key: "beginDate",
      render: (value) => <Text>{dayjs(value).fromNow()}</Text>,
      width: "20%",
    },
    {
      title: t("employees.position"),
      dataIndex: "positionRole",
      key: "positionRole",
    },
  ];

  const dataSource = orderBy(record.positions, ["beginDate"], ["desc"]);

  return record.positions.length > 1 ? (
    <div>
      <div>
        <div style={{ display: "flex", justifyContent: "flex-start" }}>
          <Button
            type="link"
            style={{ padding: 0, textAlign: "left" }}
            onClick={() => setOpened((prev) => !prev)}
          >
            {dataSource[0].positionRole}
          </Button>
        </div>
      </div>
      {opened && (
        <Modal
          open={opened}
          onOk={() => setOpened(false)}
          onCancel={() => setOpened(false)}
          style={{ minWidth: "40vw" }}
          bodyStyle={{
            maxHeight: "300px",
            overflow: "auto",
          }}
        >
          <Table
            dataSource={dataSource}
            pagination={false}
            rowKey="field"
            bordered
            size="small"
            scroll={{ x: true }}
            columns={columns}
          />
        </Modal>
      )}
    </div>
  ) : (
    record.positions[0].positionRole
  );
};
