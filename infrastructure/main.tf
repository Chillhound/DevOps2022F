terraform {
    required_providers {
        digitalocean = {
            source = "digitalocean/digitalocean"
        }
    }
}

variable do_token {}
provider digitalocean {
    token = var.do_token
}

data "digitalocean_ssh_key" "home" {
    name = "minitwit"
}

variable "region" {
    type    = string
    default = "fra1"
}

variable "droplet_count" {
    type = number
    default = 1
}

variable "droplet_size" {
    type = string
    default = "s-1vcpu-1gb"
}

resource "digitalocean_droplet" "web" {
    count = var.droplet_count
    image = "ubuntu-20-04-x64"
    name = "web-${var.region}-${count.index +1}"
    region = var.region
    size = var.droplet_size
    ssh_keys = [data.digitalocean_ssh_key.home.id]

    user_data = <<EOF
    #cloud-config
    packages:
        - nginx
    runcmd:
        - [ sh, -xc, "echo '<h1>web-${var.region}-${count.index +1}</h1>' >> /var/www/html/index.nginx-debian.html"]
    EOF

    lifecycle {
        create_before_destroy = true
    }
}
resource "digitalocean_loadbalancer" "web" {
    name        = "web-${var.region}"
    region      = var.region
    droplet_ids = digitalocean_droplet.web.*.id

    forwarding_rule {
        entry_port = 80
        entry_protocol = "http"

        target_port = 80
        target_protocol = "http"
    }

    healthcheck {
        port     = 22
        protocol = "tcp"
    }


    lifecycle {
        create_before_destroy = true
    }

    // Provisioner check to ensure loadbalancer is up and running before old one is removed
    /*provisioner "local-exec" {
        command = "./check_health.sh ${self.ipv4_address}"
    }*/
}

output "servers" {
    value = digitalocean_droplet.web.*.ipv4_address
}

output "lb" {
    value = digitalocean_loadbalancer.web.ip
}